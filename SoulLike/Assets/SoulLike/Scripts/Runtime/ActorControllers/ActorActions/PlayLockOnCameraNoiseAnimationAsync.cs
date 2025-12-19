using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using UnityEngine;

namespace SoulLike.ActorControllers.ActorActions
{
    [Serializable]
    public sealed class PlayLockOnCameraNoiseAnimationAsync : IActorAction
    {
        [SerializeField]
        private float amplitudeGainFrom;

        [SerializeField]
        private float amplitudeGainTo;

        [SerializeField]
        private float duration;

        public UniTask InvokeAsync(Actor actor, CancellationToken cancellationToken)
        {
            var worldCameraController = TinyServiceLocator.Resolve<WorldCameraController>();
            worldCameraController.PlayLockOnCameraNoiseAnimationAsync(amplitudeGainFrom, amplitudeGainTo, duration, cancellationToken).Forget();
            return UniTask.CompletedTask;
        }
    }
}
