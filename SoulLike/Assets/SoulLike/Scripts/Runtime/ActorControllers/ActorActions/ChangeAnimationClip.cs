using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SoulLike.ActorControllers.Abilities;
using UnityEngine;

namespace SoulLike.ActorControllers.ActorActions
{
    [Serializable]
    public sealed class ChangeAnimationClip : IActorAction
    {
        [SerializeField]
        private string stateName;

        [SerializeField]
        private AnimationClip animationClip;

        public UniTask InvokeAsync(Actor actor, CancellationToken cancellationToken)
        {
            actor.GetAbility<ActorAnimation>().ChangeAnimationClip(stateName, animationClip);
            return UniTask.CompletedTask;
        }
    }
}
