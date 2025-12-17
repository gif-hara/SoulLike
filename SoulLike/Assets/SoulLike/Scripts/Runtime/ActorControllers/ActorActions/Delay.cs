using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SoulLike.ActorControllers.ActorActions
{
    [Serializable]
    public sealed class Delay : IActorAction
    {
        [SerializeField]
        private float duration;

        public UniTask InvokeAsync(Actor actor, CancellationToken cancellationToken)
        {
            return UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: cancellationToken);
        }
    }
}
