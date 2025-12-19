using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SoulLike.ActorControllers.ActorActions
{
    [Serializable]
    public sealed class BeginHitStop : IActorAction
    {
        [SerializeField]
        private float duration = 0.1f;

        [SerializeField]
        private float timeScale = 0.0f;

        public UniTask InvokeAsync(Actor actor, CancellationToken cancellationToken)
        {
            return HK.Time.Root.BeginHitStopAsync(duration, timeScale, cancellationToken);
        }
    }
}
