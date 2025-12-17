using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SoulLike.ActorControllers.Abilities;
using SoulLike.MasterDataSystem;
using UnityEngine;

namespace SoulLike.ActorControllers.ActorActions
{
    [Serializable]
    public sealed class ApplyActorStatusSpec : IActorAction
    {
        [SerializeField]
        private ActorStatusSpec statusSpec;

        public UniTask InvokeAsync(Actor actor, CancellationToken cancellationToken)
        {
            actor.GetAbility<ActorStatus>().ApplySpec(statusSpec, new AdditionalStatusEmpty());
            return UniTask.CompletedTask;
        }
    }
}
