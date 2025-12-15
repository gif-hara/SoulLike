using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using R3.Triggers;
using SoulLike.MasterDataSystem;
using UnityEngine;
using UnityEngine.EventSystems;
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

        private List<UIElementList> elementLists = new();

        public void Initialize()
        {
            gameObject.SetActive(false);
        }

        public async UniTask BeginAsync(MasterData masterData, UserData userData, PlayerInput playerInput, UIViewFade uiViewFade, UIViewDialog uiViewDialog, CancellationToken cancellationToken)
        {
            using var scope = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            uiViewFade.BeginAsync(0.0f, 0.25f, scope.Token).Forget();
            CreateList(masterData, userData, uiViewDialog);
            gameObject.SetActive(true);

            await playerInput.actions["UI/Cancel"].OnPerformedAsObservable()
                .FirstAsync(scope.Token)
                .AsUniTask();
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

            foreach (var shopElement in masterData.ShopElements)
            {
                var purchasedCount = userData.GetPurchasedShopElementCount(shopElement.name);
                if (!shopElement.IsDisplayable(purchasedCount))
                {
                    continue;
                }

                var elementList = Instantiate(elementListPrefab, elementListParent);
                elementList.Setup(shopElement.Icon, string.Format(shopElement.ElementName, purchasedCount + 1));
                elementLists.Add(elementList);
                elementList.Button.OnClickAsObservable()
                    .SubscribeAwait((this, masterData, shopElement, userData, purchasedCount, uiViewDialog), static async (_, t, cts) =>
                    {
                        var (@this, masterData, shopElement, userData, purchasedCount, uiViewDialog) = t;
                        var result = await uiViewDialog.ShowAsync("購入しますか？", new string[] { "はい", "いいえ" });
                        if (result == 0)
                        {
                            foreach (var action in shopElement.Actions)
                            {
                                action.Value.Invoke(userData);
                            }
                            userData.AddPurchasedShopElementCount(shopElement.name, purchasedCount + 1);
                            @this.CreateList(masterData, userData, uiViewDialog);
                        }
                    })
                    .AddTo(elementList);
            }

            if (elementLists.Count > 0)
            {
                elementLists[0].Button.Select();
                elementLists.Select(x => x.Button).ToList().SetNavigationVertical();
            }
        }
    }
}
