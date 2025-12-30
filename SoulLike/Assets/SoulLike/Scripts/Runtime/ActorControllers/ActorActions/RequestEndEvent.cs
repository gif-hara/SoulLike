using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SoulLike.ActorControllers.ActorActions
{
    [Serializable]
    public sealed class RequestEndEvent : IActorAction
    {
        [SerializeField]
        private string tag;

        public UniTask InvokeAsync(Actor actor, CancellationToken cancellationToken)
        {
            actor.Event.Broker.Publish(new ActorEvent.RequestEndEvent(tag));
            return UniTask.CompletedTask;
        }
    }
}
