using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoulLike
{
    public class UIViewEffectMessage : MonoBehaviour
    {
        [SerializeField]
        private Image backgroundImage;

        [SerializeField]
        private Image forwardImage;

        [SerializeField]
        private TMP_Text messageText;

        [SerializeField]
        private DelayCharElement[] delayCharElements;

        public void Initialize()
        {
            backgroundImage.enabled = false;
            forwardImage.enabled = false;
            messageText.enabled = false;
        }

        public async UniTask BeginAsync(Color backgroundColor, Color forwardColor, string message, CancellationToken cancellationToken)
        {
            var backgroundColorFrom = backgroundImage.color;
            backgroundColorFrom.a = 0f;
            var forwardColorFrom = forwardImage.color;
            forwardColorFrom.a = 0f;
            backgroundImage.enabled = true;
            forwardImage.enabled = true;
            messageText.enabled = true;
            backgroundImage.color = backgroundColorFrom;
            forwardImage.color = forwardColorFrom;
            messageText.SetText(message);
            messageText.maxVisibleCharacters = 0;
            await LMotion.Create(backgroundColorFrom, backgroundColor, 0.2f)
                .BindToColor(backgroundImage)
                .ToUniTask(cancellationToken);
            while (!cancellationToken.IsCancellationRequested && messageText.maxVisibleCharacters < message.Length)
            {
                messageText.maxVisibleCharacters++;
                var delayTime = 0.05f;
                var currentChar = messageText.textInfo.characterInfo[Mathf.Clamp(messageText.maxVisibleCharacters - 1, 0, messageText.textInfo.characterCount - 1)].character;
                foreach (var delayCharElement in delayCharElements)
                {
                    if (delayCharElement.IsMatch(currentChar))
                    {
                        delayTime = delayCharElement.delayTime;
                        break;
                    }
                }
                await UniTask.Delay(TimeSpan.FromSeconds(delayTime), cancellationToken: cancellationToken);
            }
            await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: cancellationToken);
            await LMotion.Create(forwardColorFrom, forwardColor, 0.2f)
                .BindToColor(forwardImage)
                .ToUniTask(cancellationToken);
            backgroundImage.enabled = false;
            messageText.enabled = false;
            await LMotion.Create(forwardColor, forwardColorFrom, 0.2f)
                .BindToColor(forwardImage)
                .ToUniTask(cancellationToken);
        }

        [Serializable]
        public class DelayCharElement
        {
            public string targetCharacter;

            public float delayTime;

            public bool IsMatch(char character)
            {
                foreach (var c in targetCharacter)
                {
                    if (c == character)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
