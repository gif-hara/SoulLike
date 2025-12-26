using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SoulLike.ActorControllers.Abilities;
using UnityEngine;

namespace SoulLike.ActorControllers.ActorActions
{
    [Serializable]
    public sealed class PlayAnimation : IActorAction
    {
        [SerializeField]
        private string stateName;

        public UniTask InvokeAsync(Actor actor, CancellationToken cancellationToken)
        {
            actor.GetAbility<ActorAnimation>().PlayAnimation(stateName);
            return UniTask.CompletedTask;
        }
    }
}
