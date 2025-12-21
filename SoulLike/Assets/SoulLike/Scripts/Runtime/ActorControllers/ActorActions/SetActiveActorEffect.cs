using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SoulLike.ActorControllers.Abilities;
using UnityEngine;

namespace SoulLike.ActorControllers.ActorActions
{
    [Serializable]
    public sealed class SetActiveActorEffect : IActorAction
    {
        [SerializeField]
        private string effectName;

        [SerializeField]
        private bool active;

        public UniTask InvokeAsync(Actor actor, CancellationToken cancellationToken)
        {
            var actorEffect = actor.GetAbility<ActorEffect>();
            actorEffect.SetActive(effectName, active);
            return UniTask.CompletedTask;
        }
    }
}
