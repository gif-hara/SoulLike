using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SoulLike.ActorControllers.Abilities;
using UnityEngine;

namespace SoulLike.ActorControllers.AISystems.Actions
{
    [Serializable]
    public sealed class TryUniqueAttack : IAIAction
    {
        [SerializeField]
        private int uniqueAttackId;

        public async UniTask InvokeAsync(Actor actor, ActorAIController actorAIController, CancellationToken cancellationToken)
        {
            var actorWeaponHandler = actor.GetAbility<ActorWeaponHandler>();
            while (!cancellationToken.IsCancellationRequested && !actorWeaponHandler.TryUniqueAttack(uniqueAttackId))
            {
                await UniTask.Yield(cancellationToken);
            }
        }
    }
}
