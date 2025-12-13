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

        public async UniTask BeginAsync(PlayerInput playerInput, CancellationToken cancellationToken)
        {
            using var scope = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            gameObject.SetActive(true);

            await playerInput.actions["UI/Cancel"].OnPerformedAsObservable()
                .FirstAsync(scope.Token)
                .AsUniTask();

            gameObject.SetActive(false);
            scope.Cancel();
        }
    }
}
