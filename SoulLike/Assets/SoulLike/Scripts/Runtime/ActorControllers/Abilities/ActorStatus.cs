using System;
using R3;
using R3.Triggers;
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

        private ActorTime actorTime;

        private ReactiveProperty<float> hitPoint = new();

        public ReadOnlyReactiveProperty<float> HitPoint => hitPoint;

        private ReactiveProperty<float> hitPointMax = new();

        public ReadOnlyReactiveProperty<float> HitPointMax => hitPointMax;

        private ReactiveProperty<float> stamina = new();

        public ReadOnlyReactiveProperty<float> Stamina => stamina;

        private ReactiveProperty<float> staminaMax = new();

        public ReadOnlyReactiveProperty<float> StaminaMax => staminaMax;

        private const string TakeDamageStateName = "TakeDamage";

        private IDisposable endTakeDamageDisposable;

        private float staminaRecoveryPerSecond;

        public readonly Blocker StaminaRecoveryBlocker = new();

        public float HitPointRate => hitPointMax.Value > 0f ? hitPoint.Value / hitPointMax.Value : 0f;

        public float StaminaRate => staminaMax.Value > 0f ? stamina.Value / staminaMax.Value : 0f;

        public void Activate(Actor actor)
        {
            this.actor = actor;
            actorMovement = actor.GetAbility<ActorMovement>();
            actorAnimation = actor.GetAbility<ActorAnimation>();
            actorWeaponHandler = actor.GetAbility<ActorWeaponHandler>();
            actorDodge = actor.GetAbility<ActorDodge>();
            actorTime = actor.GetAbility<ActorTime>();
            actor.UpdateAsObservable()
                .Where(this, static (_, @this) => !@this.StaminaRecoveryBlocker.IsBlocked)
                .Subscribe(this, static (_, @this) =>
                {
                    if (@this.stamina.Value < @this.staminaMax.Value)
                    {
                        @this.stamina.Value = Mathf.Min(@this.staminaMax.Value, @this.stamina.Value + @this.staminaRecoveryPerSecond * @this.actorTime.Time.deltaTime);
                    }
                })
                .RegisterTo(actor.destroyCancellationToken);
        }

        public void ApplySpec(MasterDataSystem.ActorStatusSpec spec)
        {
            hitPointMax.Value = spec.HitPoint;
            hitPoint.Value = spec.HitPoint;
            actorAnimation.SetBool(ActorAnimation.Parameter.IsAlive, hitPoint.Value > 0f);
            staminaMax.Value = spec.Stamina;
            stamina.Value = spec.Stamina;
            staminaRecoveryPerSecond = spec.StaminaRecoveryPerSecond;
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

        public bool CanUseStamina()
        {
            return stamina.Value > 0f;
        }

        public void UseStamina(float amount)
        {
            stamina.Value -= amount;
        }
    }
}
