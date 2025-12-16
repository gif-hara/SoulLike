using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace SoulLike
{
    public class UIElementDamageLabel : MonoBehaviour
    {
        [SerializeField]
        private RectTransform labelRoot;

        [SerializeField]
        private TMP_Text label;

        [SerializeField]
        private float destroyDelay = 1.0f;

        public void Setup(string text, Vector3 worldPosition, Camera worldCamera)
        {
            SetupAsync(text, worldPosition, worldCamera, destroyCancellationToken).Forget();
        }

        private async UniTask SetupAsync(string text, Vector3 worldPosition, Camera worldCamera, CancellationToken cancellationToken)
        {
            label.text = text;
            var screenPosition = worldCamera.WorldToScreenPoint(worldPosition);
            labelRoot.position = screenPosition;

            await UniTask.Delay(TimeSpan.FromSeconds(destroyDelay), cancellationToken: cancellationToken);
            Destroy(gameObject);
        }
    }
}
