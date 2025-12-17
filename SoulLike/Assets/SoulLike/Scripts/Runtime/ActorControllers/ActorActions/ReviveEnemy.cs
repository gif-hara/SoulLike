using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SoulLike.ActorControllers.AISystems;
using SoulLike.MasterDataSystem;
using UnityEngine;

namespace SoulLike.ActorControllers.ActorActions
{
    [Serializable]
    public sealed class ReviveEnemy : IActorAction
    {
        [SerializeField]
        private ActorStatusSpec actorStatusSpec;

        [SerializeField]
        private ActorAI newAI;

        public UniTask InvokeAsync(Actor actor, CancellationToken cancellationToken)
        {
            actor.Event.Broker.Publish(new ActorEvent.ReviveEnemy(actorStatusSpec, newAI));
            return UniTask.CompletedTask;
        }
    }
}
