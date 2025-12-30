using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SoulLike.ActorControllers.ActorActions
{
    [Serializable]
    public sealed class RequestBeginEvent : IActorAction
    {
        [SerializeField]
        private string tag;

        public UniTask InvokeAsync(Actor actor, CancellationToken cancellationToken)
        {
            actor.Event.Broker.Publish(new ActorEvent.RequestBeginEvent(tag));
            return UniTask.CompletedTask;
        }
    }
}
