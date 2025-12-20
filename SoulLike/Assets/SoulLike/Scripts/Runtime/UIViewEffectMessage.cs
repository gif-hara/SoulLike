using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using SoulLike.ActorControllers;
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

        public async UniTask BeginAsync(Color backgroundColor, Color forwardColor, string message, IMessagePublisher publisher, CancellationToken cancellationToken)
        {
            var backgroundColorFrom = backgroundImage.color;
            backgroundColorFrom.a = 0f;
            var forwardColorFrom = forwardImage.color;
            forwardColorFrom.a = 0f;
            Setup(backgroundColorFrom, forwardColorFrom, message);
            await LMotion.Create(backgroundColorFrom, backgroundColor, 1.0f)
                .BindToColor(backgroundImage)
                .ToUniTask(cancellationToken);
            await PlayMessageAnimationAsync(message, cancellationToken);
            await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: cancellationToken);
            await LMotion.Create(forwardColorFrom, forwardColor, 1.0f)
                .BindToColor(forwardImage)
                .ToUniTask(cancellationToken);
            backgroundImage.enabled = false;
            messageText.enabled = false;
            await LMotion.Create(forwardColor, forwardColorFrom, 1.0f)
                .BindToColor(forwardImage)
                .ToUniTask(cancellationToken);
            publisher.Publish(new ActorEvent.OnCompleteEffectMessage());
        }

        private void Setup(Color backgroundColor, Color forwardColor, string message)
        {
            backgroundImage.enabled = true;
            forwardImage.enabled = true;
            messageText.enabled = true;
            backgroundImage.color = backgroundColor;
            forwardImage.color = forwardColor;
            messageText.SetText(message);
            messageText.maxVisibleCharacters = 0;
        }

        private async UniTask PlayMessageAnimationAsync(string message, CancellationToken cancellationToken)
        {
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
