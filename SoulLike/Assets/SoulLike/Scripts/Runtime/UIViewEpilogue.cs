using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using LitMotion;
using LitMotion.Extensions;
using R3;
using TMPro;
using TNRD;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SoulLike
{
    public class UIViewEpilogue : MonoBehaviour
    {
        [SerializeField]
        private Image image;

        [SerializeField]
        private CanvasGroup imageCanvasGroup;

        [SerializeField]
        private TMP_Text messageCenter;

        [SerializeField]
        private TMP_Text messageBottom;

        [SerializeField]
        private GameObject areaSkip;

        [SerializeField]
        private TMP_Text skipMessage;

        [SerializeField]
        private string skipMessageFormat;

#if UNITY_EDITOR
        [ClassesOnly]
#endif
        [SerializeField]
        private List<SerializableInterface<IAction>> actions;

        public enum ResultType
        {
            EndActions,
            Skip
        }

        public enum MessagePositionType
        {
            Center,
            Bottom
        }

        public void Initialize()
        {
            gameObject.SetActive(false);
        }

        public async UniTask<ResultType> BeginAsync(PlayerInput playerInput, CancellationToken cancellationToken)
        {
            using var scope = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            gameObject.SetActive(true);
            areaSkip.SetActive(false);
            imageCanvasGroup.alpha = 0.0f;
            var result = await UniTask.WhenAny(
                ProcessSkipAsync(playerInput, scope.Token),
                BeginActionsAsync(scope.Token)
            );

            scope.Cancel();
            return result == 0 ? ResultType.Skip : ResultType.EndActions;
        }

        private async UniTask BeginActionsAsync(CancellationToken cancellationToken)
        {
            foreach (var action in actions)
            {
                await action.Value.InvokeAsync(this, cancellationToken);
            }
        }

        private async UniTask ProcessSkipAsync(PlayerInput playerInput, CancellationToken cancellationToken)
        {
            await InputSystem.onAnyButtonPress.ToObservable().FirstAsync(cancellationToken);
            await UniTask.NextFrame(cancellationToken);
            var action = playerInput.actions["Submit"];
            skipMessage.SetText(string.Format(skipMessageFormat, InputSprite.GetTag(playerInput, action)));
            areaSkip.SetActive(true);
            await action.OnPerformedAsObservable().FirstAsync(cancellationToken);
            areaSkip.SetActive(false);
        }

        public async UniTask PlayMessageAnimationAsync(string message, MessagePositionType positionType, float visibleDuration, float delayTime, CancellationToken cancellationToken)
        {
            messageCenter.enabled = positionType == MessagePositionType.Center;
            messageBottom.enabled = positionType == MessagePositionType.Bottom;
            var messageText = positionType == MessagePositionType.Center ? messageCenter : messageBottom;
            messageText.text = message;
            messageText.maxVisibleCharacters = 0;
            messageText.enabled = true;
            while (!cancellationToken.IsCancellationRequested && messageText.maxVisibleCharacters < message.Length)
            {
                messageText.maxVisibleCharacters++;
                await UniTask.Delay(TimeSpan.FromSeconds(delayTime), cancellationToken: cancellationToken);
            }
            await UniTask.Delay(TimeSpan.FromSeconds(visibleDuration), cancellationToken: cancellationToken);
        }

        public void PlayImageAnimation(Sprite sprite)
        {
            image.sprite = sprite;
            imageCanvasGroup.alpha = 0.0f;
            LMotion.Create(0.0f, 1.0f, 1.0f)
                .BindToAlpha(imageCanvasGroup)
                .AddTo(this);
        }

        public void HideImageAnimationAsync(CancellationToken cancellationToken)
        {
            LMotion.Create(1.0f, 0.0f, 1.0f)
                .BindToAlpha(imageCanvasGroup)
                .AddTo(this);
        }

        public interface IAction
        {
            UniTask InvokeAsync(UIViewEpilogue view, CancellationToken cancellationToken);
        }

        [Serializable]
        public sealed class PlayMessageAnimationAsyncAction : IAction
        {
            [SerializeField, Multiline]
            private string message;

            [SerializeField]
            private MessagePositionType positionType;

            [SerializeField]
            private float visibleDuration;

            [SerializeField]
            private float delayTime;

            public UniTask InvokeAsync(UIViewEpilogue view, CancellationToken cancellationToken)
            {
                return view.PlayMessageAnimationAsync(message, positionType, visibleDuration, delayTime, cancellationToken);
            }
        }

        [Serializable]
        public sealed class PlayImageAnimationAction : IAction
        {
            [SerializeField]
            private Sprite sprite;

            public UniTask InvokeAsync(UIViewEpilogue view, CancellationToken cancellationToken)
            {
                view.PlayImageAnimation(sprite);
                return UniTask.CompletedTask;
            }
        }

        [Serializable]
        public sealed class DelayAction : IAction
        {
            [SerializeField]
            private float delaySeconds;

            public async UniTask InvokeAsync(UIViewEpilogue view, CancellationToken cancellationToken)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken: cancellationToken);
            }
        }
    }
}
