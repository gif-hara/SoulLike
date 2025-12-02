using System.Threading;
using R3;
using UnityEngine;

namespace SoulLike.ActorControllers.Abilities
{
    public sealed class ActorAttack : IActorAbility
    {
        private ActorMovement actorMovement;

        private ActorAnimation actorAnimation;

        private int weaponId = 1;

        private int attackId = 1;

        public readonly ReactiveProperty<bool> CanAttack = new(true);

        private CancellationTokenSource attackingScope;

        public void Activate(Actor actor)
        {
            actorMovement = actor.GetAbility<ActorMovement>();
            actorAnimation = actor.GetAbility<ActorAnimation>();
        }

        public bool TryAttack()
        {
            if (!CanAttack.Value)
            {
                return false;
            }

            attackingScope?.Cancel();
            attackingScope?.Dispose();
            attackingScope = new CancellationTokenSource();
            actorAnimation.SetInteger(ActorAnimation.Parameter.AttackId, attackId);
            actorAnimation.SetTrigger(ActorAnimation.Parameter.Attack);
            CanAttack.Value = false;
            var currentAttackId = attackId;
            attackId++;
            actorAnimation.UpdateAnimator();
            actorAnimation.OnStateExitAsObservable()
                .Subscribe((this, currentAttackId), static (x, t) =>
                {
                    var (@this, currentAttackId) = t;
                    if (x.StateInfo.IsName(ActorAnimation.Parameter.GetAttackStateName(@this.weaponId, currentAttackId)))
                    {
                        @this.actorMovement.CanMove.Value = true;
                        @this.CanAttack.Value = true;
                        @this.attackId = 1;
                        @this.attackingScope?.Cancel();
                        @this.attackingScope?.Dispose();
                        @this.attackingScope = null;
                        Debug.Log($"Attack finished");
                    }
                })
                .RegisterTo(attackingScope.Token);
            return true;
        }
    }
}
