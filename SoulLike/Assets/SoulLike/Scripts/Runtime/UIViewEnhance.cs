using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SoulLike
{
    public class UIViewEnhance : MonoBehaviour
    {
        [SerializeField]
        private UIElementList elementListPrefab;

        public void Initialize()
        {
            gameObject.SetActive(false);
        }

        public async UniTask BeginAsync(PlayerInput playerInput, UIViewFade uiViewFade, CancellationToken cancellationToken)
        {
            using var scope = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            uiViewFade.BeginAsync(0.0f, 0.25f, scope.Token).Forget();
            gameObject.SetActive(true);

            await playerInput.actions["UI/Cancel"].OnPerformedAsObservable()
                .FirstAsync(scope.Token)
                .AsUniTask();
            await uiViewFade.BeginAsync(1.0f, 0.25f, scope.Token);

            gameObject.SetActive(false);
            scope.Cancel();
        }
    }
}
