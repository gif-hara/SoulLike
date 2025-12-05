using System;
using System.Threading;
using R3;
using UnityEngine;

namespace SoulLike.ActorControllers.Abilities
{
    public sealed class ActorWeaponHandler : IActorAbility
    {
        private Actor actor;

        private ActorMovement actorMovement;

        private ActorAnimation actorAnimation;

        private int weaponId = 1;

        private int attackId = 1;

        public readonly ReactiveProperty<bool> CanAttack = new(true);

        private IDisposable attackExitProcess;

        public void Activate(Actor actor)
        {
            this.actor = actor;
            actorMovement = actor.GetAbility<ActorMovement>();
            actorAnimation = actor.GetAbility<ActorAnimation>();
        }

        public void CreateWeapon(Weapon weaponPrefab)
        {
            var weaponObject = UnityEngine.Object.Instantiate(weaponPrefab);
            weaponObject.Initialize(actor, this);
        }

        public bool TryAttack()
        {
            if (!CanAttack.Value)
            {
                return false;
            }

            attackExitProcess?.Dispose();
            attackExitProcess = null;
            actorAnimation.SetInteger(ActorAnimation.Parameter.AttackId, attackId);
            actorAnimation.SetTrigger(ActorAnimation.Parameter.Attack);
            CanAttack.Value = false;
            var currentAttackId = attackId;
            attackId++;
            actorAnimation.UpdateAnimator();
            attackExitProcess = actorAnimation.OnStateExitAsObservable()
                .Subscribe((this, currentAttackId), static (x, t) =>
                {
                    var (@this, currentAttackId) = t;
                    if (x.StateInfo.IsName(ActorAnimation.Parameter.GetAttackStateName(@this.weaponId, currentAttackId)))
                    {
                        @this.actorMovement.CanMove.Value = true;
                        @this.CanAttack.Value = true;
                        @this.attackId = 1;
                        @this.attackExitProcess?.Dispose();
                        @this.attackExitProcess = null;
                        Debug.Log($"Attack finished");
                    }
                });
            return true;
        }
    }
}
