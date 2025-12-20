using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using UnityEngine;

namespace SoulLike.ActorControllers.ActorActions
{
    [Serializable]
    public sealed class FadeOutBgm : IActorAction
    {
        [SerializeField]
        private float duration;

        [SerializeField]
        private float targetVolume = 0f;

        [SerializeField]
        private bool isForget;

        public UniTask InvokeAsync(Actor actor, CancellationToken cancellationToken)
        {
            if (isForget)
            {
                TinyServiceLocator.Resolve<AudioManager>().FadeOutBgmAsync(duration, targetVolume, cancellationToken).Forget();
                return UniTask.CompletedTask;
            }
            return TinyServiceLocator.Resolve<AudioManager>().FadeOutBgmAsync(duration, targetVolume, cancellationToken);
        }
    }
}
