using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using HK;
using LitMotion;
using LitMotion.Extensions;
using R3;
using R3.Triggers;
using SoulLike.MasterDataSystem;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SoulLike
{
    public class UIViewEnhance : MonoBehaviour
    {
        [SerializeField]
        private Transform elementListParent;

        [SerializeField]
        private UIElementList elementListPrefab;

        [SerializeField]
        private TMP_Text experienceText;

        [SerializeField]
        private TMP_Text hintMessageText;

        [SerializeField]
        private CanvasGroup hintMessageCanvasGroup;

        [SerializeField, Multiline]
        private string[] hintMessages;

        private readonly List<UIElementList> elementLists = new();

        private List<ShopElement> availableShopElements = new();

        private int selectedIndex;

        private int hintMessageIndex;

        public void Initialize()
        {
            gameObject.SetActive(false);
        }

        public async UniTask BeginAsync(MasterData masterData, UserData userData, PlayerInput playerInput, UIViewFade uiViewFade, UIViewDialog uiViewDialog, bool isShowTutorial, CancellationToken cancellationToken)
        {
            using var scope = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var audioManager = TinyServiceLocator.Resolve<AudioManager>();
            userData.Experience
                .Subscribe(this, static (x, @this) =>
                {
                    @this.experienceText.SetText(x.ToString());
                })
                .RegisterTo(scope.Token);
            uiViewFade.BeginAsync(0.0f, 0.25f, scope.Token).Forget();
            CreateList(masterData, userData, audioManager);
            gameObject.SetActive(true);

            var cancelAction = playerInput.actions["UI/Cancel"];

            if (isShowTutorial)
            {
                var message = string.Format(
                    "諦めるな！<color=#DDAA00>経験値</color>と引き換えに能力を得よ！{0}無事、師を乗り越えよ！",
                    Environment.NewLine
                );
                await uiViewDialog.ShowAsync(message, new string[] { "OK" }, cancelAction, 0, scope.Token);
                audioManager.PlaySfx("Decide.1");
                elementLists[selectedIndex].Button.Select();
            }

            BeginHintMessageAsync(scope.Token).Forget();

            while (!scope.Token.IsCancellationRequested)
            {
                var elementListResult = elementLists.Count > 0
                    ? UniTask.WhenAny(elementLists.Select(x => x.Button.OnClickAsObservable().FirstAsync(scope.Token).AsUniTask()))
                    : UniTask.Never<(int winArgumentIndex, Unit result)>(scope.Token);
                var result = await UniTask.WhenAny(
                    elementListResult,
                    cancelAction.OnPerformedAsObservable().FirstAsync(scope.Token).AsUniTask()
                );
                if (result.winArgumentIndex == 0)
                {
                    var shopElement = availableShopElements[result.result1.winArgumentIndex];
                    var purchasedCount = userData.GetPurchasedShopElementCount(shopElement.name);
                    var price = shopElement.GetPrice(purchasedCount);
                    if (userData.Experience.CurrentValue < price)
                    {
                        audioManager.PlaySfx("Abort.1");
                        await uiViewDialog.ShowAsync("経験値が足りません。", new string[] { "OK" }, cancelAction, 0, scope.Token);
                        audioManager.PlaySfx("Cancel.1");
                        elementLists[selectedIndex].Button.Select();
                        continue;
                    }
                    audioManager.PlaySfx("Select.1");
                    var purchaseResult = await uiViewDialog.ShowAsync("会得しますか？", new string[] { "はい", "いいえ" }, cancelAction, 1, scope.Token);
                    if (purchaseResult == 0)
                    {
                        foreach (var action in shopElement.Actions)
                        {
                            action.Value.Invoke(userData);
                        }
                        audioManager.PlaySfx("Decide.1");
                        userData.AddExperience(-price);
                        userData.AddPurchasedShopElementCount(shopElement.name, purchasedCount + 1);
                        CreateList(masterData, userData, audioManager);
                    }
                    else
                    {
                        audioManager.PlaySfx("Cancel.1");
                        elementLists[selectedIndex].Button.Select();
                    }
                }
                else
                {
                    audioManager.PlaySfx("Select.1");
                    var cancelResult = await uiViewDialog.ShowAsync("再戦します。よろしいですか？", new string[] { "はい", "いいえ" }, cancelAction, 1, scope.Token);
                    if (cancelResult == 0)
                    {
                        audioManager.PlaySfx("Decide.2");
                        break;
                    }
                    else
                    {
                        audioManager.PlaySfx("Cancel.1");
                        if (elementLists.Count > 0)
                        {
                            elementLists[selectedIndex].Button.Select();
                        }
                    }
                }
            }
            TinyServiceLocator.Resolve<AudioManager>().FadeOutBgmAsync(0.25f, 0.0f, scope.Token).Forget();
            await uiViewFade.BeginAsync(1.0f, 0.25f, scope.Token);

            gameObject.SetActive(false);
            scope.Cancel();
        }

        private void CreateList(MasterData masterData, UserData userData, AudioManager audioManager)
        {
            foreach (var elementList in elementLists)
            {
                Destroy(elementList.gameObject);
            }
            elementLists.Clear();
            availableShopElements = masterData.ShopElements.Where(x => x.IsDisplayable(userData.GetPurchasedShopElementCount(x.name))).ToList();

            for (int i = 0; i < availableShopElements.Count; i++)
            {
                var shopElement = availableShopElements[i];
                var purchasedCount = userData.GetPurchasedShopElementCount(shopElement.name);
                var elementList = Instantiate(elementListPrefab, elementListParent);
                elementList.Setup(shopElement.Icon, shopElement.IconColor, string.Format(shopElement.ElementName, purchasedCount + 1), shopElement.GetPrice(purchasedCount).ToString());
                elementLists.Add(elementList);
                elementList.Button.OnSelectAsObservable()
                    .Subscribe((this, i, audioManager), static (_, t) =>
                    {
                        var (@this, i, audioManager) = t;
                        @this.selectedIndex = i;
                        audioManager.PlaySfx("Select.1");
                    })
                    .AddTo(elementList);
            }

            if (elementLists.Count > 0)
            {
                var firstElement = elementLists[Mathf.Clamp(selectedIndex, 0, elementLists.Count - 1)];
                firstElement.Button.Select();
                elementLists.Select(x => x.Button).ToList().SetNavigationVertical();
            }
        }

        private async UniTask BeginHintMessageAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                hintMessageText.SetText(hintMessages[hintMessageIndex]);
                await LMotion.Create(0.0f, 1.0f, 0.5f)
                    .BindToAlpha(hintMessageCanvasGroup)
                    .ToUniTask(cancellationToken: cancellationToken);
                await UniTask.Delay(TimeSpan.FromSeconds(5.0f), cancellationToken: cancellationToken);
                await LMotion.Create(1.0f, 0.0f, 0.5f)
                    .BindToAlpha(hintMessageCanvasGroup)
                    .ToUniTask(cancellationToken: cancellationToken);
                hintMessageIndex = (hintMessageIndex + 1) % hintMessages.Length;
            }
        }
    }
}
