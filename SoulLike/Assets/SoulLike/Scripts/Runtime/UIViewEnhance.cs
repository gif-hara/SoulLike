using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using R3.Triggers;
using SoulLike.MasterDataSystem;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

        private readonly List<UIElementList> elementLists = new();

        private List<ShopElement> availableShopElements = new();

        private int selectedIndex;

        public void Initialize()
        {
            gameObject.SetActive(false);
        }

        public async UniTask BeginAsync(MasterData masterData, UserData userData, PlayerInput playerInput, UIViewFade uiViewFade, UIViewDialog uiViewDialog, CancellationToken cancellationToken)
        {
            using var scope = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            userData.Experience
                .Subscribe(this, static (x, @this) =>
                {
                    @this.experienceText.SetText(x.ToString());
                })
                .RegisterTo(scope.Token);
            uiViewFade.BeginAsync(0.0f, 0.25f, scope.Token).Forget();
            CreateList(masterData, userData, uiViewDialog);
            gameObject.SetActive(true);
            var cancelAction = playerInput.actions["UI/Cancel"];

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
                        await uiViewDialog.ShowAsync("経験値が足りません。", new string[] { "OK" }, cancelAction, 0, scope.Token);
                        elementLists[selectedIndex].Button.Select();
                        continue;
                    }
                    var purchaseResult = await uiViewDialog.ShowAsync("購入しますか？", new string[] { "はい", "いいえ" }, cancelAction, 1, scope.Token);
                    if (purchaseResult == 0)
                    {
                        foreach (var action in shopElement.Actions)
                        {
                            action.Value.Invoke(userData);
                        }
                        userData.AddExperience(-price);
                        userData.AddPurchasedShopElementCount(shopElement.name, purchasedCount + 1);
                        CreateList(masterData, userData, uiViewDialog);
                    }
                    else
                    {
                        elementLists[selectedIndex].Button.Select();
                    }
                }
                else
                {
                    var cancelResult = await uiViewDialog.ShowAsync("再戦します。よろしいですか？", new string[] { "はい", "いいえ" }, cancelAction, 1, scope.Token);
                    if (cancelResult == 0)
                    {
                        break;
                    }
                    else
                    {
                        if (elementLists.Count > 0)
                        {
                            elementLists[selectedIndex].Button.Select();
                        }
                    }
                }
            }
            await uiViewFade.BeginAsync(1.0f, 0.25f, scope.Token);

            gameObject.SetActive(false);
            scope.Cancel();
        }

        public void CreateList(MasterData masterData, UserData userData, UIViewDialog uiViewDialog)
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
                elementList.Setup(shopElement.Icon, string.Format(shopElement.ElementName, purchasedCount + 1), shopElement.GetPrice(purchasedCount).ToString());
                elementLists.Add(elementList);
                elementList.Button.OnSelectAsObservable()
                    .Subscribe((this, i), static (_, t) =>
                    {
                        var (@this, i) = t;
                        @this.selectedIndex = i;
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
    }
}
