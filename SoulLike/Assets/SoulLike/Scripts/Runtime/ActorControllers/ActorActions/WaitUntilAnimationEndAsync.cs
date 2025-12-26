using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using SoulLike.ActorControllers.Abilities;
using UnityEngine;

namespace SoulLike.ActorControllers.ActorActions
{
    [Serializable]
    public sealed class WaitUntilAnimationEndAsync : IActorAction
    {
        [SerializeField]
        private string stateName;

        public UniTask InvokeAsync(Actor actor, CancellationToken cancellationToken)
        {
            return actor.GetAbility<ActorAnimation>().WaitUntilAnimationEndAsync(cancellationToken);
        }
    }
}
