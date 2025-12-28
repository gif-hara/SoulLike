using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SoulLike.ActorControllers.AISystems;
using UnityEngine;

namespace SoulLike.ActorControllers.ActorActions
{
    [Serializable]
    public sealed class ChangeAI : IActorAction
    {
        [SerializeField]
        private ActorAI newAI;

        public UniTask InvokeAsync(Actor actor, CancellationToken cancellationToken)
        {
            actor.Event.Broker.Publish(new ActorEvent.ChangeAI(newAI));
            return UniTask.CompletedTask;
        }
    }
}
