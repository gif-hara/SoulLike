using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;

namespace SoulLike
{
    public class UIElementDamageLabel : MonoBehaviour
    {
        [SerializeField]
        private RectTransform labelRoot;

        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private TMP_Text label;

        [SerializeField]
        private float destroyDelay = 1.0f;

        [SerializeField]
        private Color defaultColor = Color.white;

        [SerializeField]
        private Color stunnedColor = Color.yellow;

        [SerializeField]
        private float defaultFontSize = 36.0f;

        [SerializeField]
        private float stunnedFontSize = 48.0f;

        public void Setup(string text, bool isStunned, Vector3 worldPosition, Camera worldCamera)
        {
            SetupAsync(text, isStunned, worldPosition, worldCamera, destroyCancellationToken).Forget();
        }

        private async UniTask SetupAsync(string text, bool isStunned, Vector3 worldPosition, Camera worldCamera, CancellationToken cancellationToken)
        {
            label.text = text;
            label.color = isStunned ? stunnedColor : defaultColor;
            label.fontSize = isStunned ? stunnedFontSize : defaultFontSize;
            var screenPosition = worldCamera.WorldToScreenPoint(worldPosition);
            labelRoot.position = screenPosition;

            if (isStunned)
            {
                await LSequence.Create()
                    .AppendInterval(0.75f)
                    .Append(LMotion.Create(1.0f, 0.0f, 0.25f).BindToAlpha(canvasGroup))
                    .Join(LMotion.Create(labelRoot.localPosition, labelRoot.localPosition + Vector3.up * 50.0f, 0.25f).BindToLocalPosition(labelRoot))
                    .Run()
                    .ToUniTask(cancellationToken: cancellationToken);
            }
            else
            {
                await LSequence.Create()
                    .AppendInterval(0.5f)
                    .Append(LMotion.Create(1.0f, 0.0f, 0.25f).BindToAlpha(canvasGroup))
                    .Run()
                    .ToUniTask(cancellationToken: cancellationToken);
            }

            Destroy(gameObject);
        }
    }
}
