using System;
using R3;
using UnityEngine;

namespace SoulLike.ActorControllers.Abilities
{
    public class ActorStatus : IActorAbility
    {
        private Actor actor;

        private ActorMovement actorMovement;

        private ActorAnimation actorAnimation;

        private ActorWeaponHandler actorWeaponHandler;

        private ActorDodge actorDodge;

        private ReactiveProperty<float> hitPoint = new();

        private ReactiveProperty<float> hitPointMax = new();

        public ReadOnlyReactiveProperty<float> HitPoint => hitPoint;

        public ReadOnlyReactiveProperty<float> HitPointMax => hitPointMax;

        private const string TakeDamageStateName = "TakeDamage";

        private IDisposable endTakeDamageDisposable;

        public float HitPointRate => hitPointMax.Value > 0f ? hitPoint.Value / hitPointMax.Value : 0f;

        public void Activate(Actor actor)
        {
            this.actor = actor;
            actorMovement = actor.GetAbility<ActorMovement>();
            actorAnimation = actor.GetAbility<ActorAnimation>();
            actorWeaponHandler = actor.GetAbility<ActorWeaponHandler>();
            actorDodge = actor.GetAbility<ActorDodge>();
        }

        public void ApplySpec(MasterDataSystem.ActorStatusSpec spec)
        {
            hitPointMax.Value = spec.HitPoint;
            hitPoint.Value = spec.HitPoint;
            actorAnimation.SetBool(ActorAnimation.Parameter.IsAlive, hitPoint.Value > 0f);
        }

        public void TakeDamage(Actor attacker, AttackData attackData)
        {
            if (hitPoint.Value <= 0f)
            {
                return;
            }
            hitPoint.Value = Mathf.Max(0f, hitPoint.Value - attackData.Power);
            actorAnimation.SetBool(ActorAnimation.Parameter.IsAlive, hitPoint.Value > 0f);
            if (hitPoint.Value <= 0f)
            {
                actorAnimation.SetTrigger(ActorAnimation.Parameter.Dead);
                actor.Event.Broker.Publish(new ActorEvent.OnDead());
            }
            else
            {
                actorAnimation.PlayDamageAnimation(attackData.DamageId);
            }

            actorMovement.MoveBlocker.Block(TakeDamageStateName);
            actorMovement.RotateBlocker.Block(TakeDamageStateName);
            actorWeaponHandler.AttackBlocker.Block(TakeDamageStateName);
            actorDodge.DodgeBlocker.Block(TakeDamageStateName);
            endTakeDamageDisposable?.Dispose();
            endTakeDamageDisposable = actorAnimation.OnStateEnterAsObservable(ActorAnimation.Parameter.Idle)
                .Subscribe(this, static (x, @this) =>
                {
                    @this.actorMovement.MoveBlocker.Unblock(TakeDamageStateName);
                    @this.actorMovement.RotateBlocker.Unblock(TakeDamageStateName);
                    @this.actorWeaponHandler.AttackBlocker.Unblock(TakeDamageStateName);
                    @this.actorDodge.DodgeBlocker.Unblock(TakeDamageStateName);
                });
        }
    }
}
