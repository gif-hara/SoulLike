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

        public async UniTask BeginAsync(Color backgroundColor, Color forwardColor, Color messageColor, string message, IMessagePublisher publisher, CancellationToken cancellationToken, Action onFirstFadeOutAction = null, Action onEndFadeOutAction = null)
        {
            var backgroundColorFrom = backgroundColor;
            backgroundColorFrom.a = 0f;
            var forwardColorFrom = forwardColor;
            forwardColorFrom.a = 0f;
            Setup(backgroundColorFrom, forwardColorFrom, messageColor, message);
            await LMotion.Create(backgroundColorFrom, backgroundColor, 1.0f)
                .BindToColor(backgroundImage)
                .ToUniTask(cancellationToken);
            onFirstFadeOutAction?.Invoke();
            await UniTask.Delay(TimeSpan.FromSeconds(2.0f), cancellationToken: cancellationToken);
            await PlayMessageAnimationAsync(message, cancellationToken);
            await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: cancellationToken);
            await LMotion.Create(forwardColorFrom, forwardColor, 1.0f)
                .BindToColor(forwardImage)
                .ToUniTask(cancellationToken);
            backgroundImage.enabled = false;
            messageText.enabled = false;
            onEndFadeOutAction?.Invoke();
            await LMotion.Create(forwardColor, forwardColorFrom, 1.0f)
                .BindToColor(forwardImage)
                .ToUniTask(cancellationToken);
            publisher.Publish(new ActorEvent.OnCompleteEffectMessage());
        }

        private void Setup(Color backgroundColor, Color forwardColor, Color messageColor, string message)
        {
            backgroundImage.enabled = true;
            forwardImage.enabled = true;
            messageText.enabled = true;
            backgroundImage.color = backgroundColor;
            forwardImage.color = forwardColor;
            messageText.SetText(message);
            messageText.maxVisibleCharacters = 0;
            messageText.color = messageColor;
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
