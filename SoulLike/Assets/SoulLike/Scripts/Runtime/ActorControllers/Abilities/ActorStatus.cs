using System;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using R3;
using R3.Triggers;
using SoulLike.ActorControllers.ActorActions;
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

        private ActorSceneViewHandler actorSceneViewHandler;

        private ReactiveProperty<float> hitPoint = new();

        public ReadOnlyReactiveProperty<float> HitPoint => hitPoint;

        private ReactiveProperty<float> hitPointMax = new();

        public ReadOnlyReactiveProperty<float> HitPointMax => hitPointMax;

        private ReactiveProperty<float> stamina = new();

        public ReadOnlyReactiveProperty<float> Stamina => stamina;

        private ReactiveProperty<float> staminaMax = new();

        public ReadOnlyReactiveProperty<float> StaminaMax => staminaMax;

        private ReactiveProperty<float> stunResistance = new();

        public ReadOnlyReactiveProperty<float> StunResistance => stunResistance;

        private ReactiveProperty<float> stunResistanceMax = new();

        public ReadOnlyReactiveProperty<float> StunResistanceMax => stunResistanceMax;

        private ReactiveProperty<float> specialPower = new();

        public ReadOnlyReactiveProperty<float> SpecialPower => specialPower;

        private ReactiveProperty<int> specialStock = new();

        public ReadOnlyReactiveProperty<int> SpecialStock => specialStock;

        private ReactiveProperty<int> specialStockMax = new();

        public ReadOnlyReactiveProperty<int> SpecialStockMax => specialStockMax;

        public float StunDuration { get; private set; }

        private ActorAction onStunAction;

        private ActorAction onSpecialStockReached;

        private const string TakeDamageStateName = "TakeDamage";

        private IDisposable endTakeDamageDisposable;

        private float staminaRecoveryPerSecond;

        public readonly Blocker StaminaRecoveryBlocker = new();

        private Blocker invincibleBlocker = new();

        private ReactiveProperty<bool> isInvincible = new(false);

        public ReadOnlyReactiveProperty<bool> IsInvincible => isInvincible;

        public bool IsParrying { get; set; } = false;

        public bool IsStunned { get; private set; } = false;

        public bool IsDead => hitPoint.Value <= 0f;

        public float HitPointRate => hitPointMax.Value > 0f ? hitPoint.Value / hitPointMax.Value : 0f;

        public float StaminaRate => staminaMax.Value > 0f ? stamina.Value / staminaMax.Value : 0f;

        public float StunResistanceRate => stunResistanceMax.Value > 0f ? stunResistance.Value / stunResistanceMax.Value : 0f;

        private IAdditionalStatus additionalStatus;

        private float attackBuffTimer = 0.0f;

        private float attackBuffRate = 0.0f;

        private float defenseDebuffRateOnStunned = 0.0f;

        private ReactiveProperty<bool> attackBuffAvailable = new(false);

        public ReadOnlyReactiveProperty<bool> AttackBuffAvailable => attackBuffAvailable;

        public float ExperienceRate { get; private set; }

        public void Activate(Actor actor)
        {
            this.actor = actor;
            actorMovement = actor.GetAbility<ActorMovement>();
            actorAnimation = actor.GetAbility<ActorAnimation>();
            actorWeaponHandler = actor.GetAbility<ActorWeaponHandler>();
            actorDodge = actor.GetAbility<ActorDodge>();
            actorTime = actor.GetAbility<ActorTime>();
            actorSceneViewHandler = actor.GetAbility<ActorSceneViewHandler>();
            actor.UpdateAsObservable()
                .Subscribe(this, static (_, @this) =>
                {
                    if (!@this.StaminaRecoveryBlocker.IsBlocked)
                    {
                        if (@this.stamina.Value < @this.staminaMax.Value)
                        {
                            @this.stamina.Value = Mathf.Min(@this.staminaMax.Value, @this.stamina.Value + @this.staminaRecoveryPerSecond * @this.actorTime.Time.deltaTime);
                        }
                    }
                    @this.attackBuffTimer -= @this.actorTime.Time.deltaTime;
                    @this.attackBuffAvailable.Value = @this.attackBuffTimer > 0.0f;
                })
                .RegisterTo(actor.destroyCancellationToken);
        }

        public void ApplySpec(MasterDataSystem.ActorStatusSpec spec, IAdditionalStatus additionalStatus)
        {
            this.additionalStatus = additionalStatus;
            hitPointMax.Value = spec.HitPoint + additionalStatus.HitPoint;
            hitPoint.Value = spec.HitPoint + additionalStatus.HitPoint;
            actorAnimation.SetBool(ActorAnimation.Parameter.IsAlive, hitPoint.Value > 0f);
            staminaMax.Value = spec.Stamina + additionalStatus.Stamina;
            stamina.Value = spec.Stamina + additionalStatus.Stamina;
            staminaRecoveryPerSecond = spec.StaminaRecoveryPerSecond + additionalStatus.StaminaRecoveryPerSecond;
            stunResistance.Value = 0;
            stunResistanceMax.Value = spec.StunResistance;
            StunDuration = spec.StunDuration;
            onStunAction = spec.OnStunAction;
            specialPower.Value = 0.0f;
            specialStock.Value = 0;
            specialStockMax.Value = spec.SpecialStockMax;
            onSpecialStockReached = spec.OnSpecialStockReached;
            attackBuffTimer = 0.0f;
            attackBuffRate = spec.AttackBuffRate;
            defenseDebuffRateOnStunned = spec.DefenseDebuffRateOnStunned;
            ExperienceRate = spec.ExperienceRate;
        }

        public TakeDamageData TakeDamage(Actor attacker, AttackData attackData)
        {
            if (hitPoint.Value <= 0f)
            {
                return new TakeDamageData(0);
            }
            var attackerStatus = attacker.GetAbility<ActorStatus>();
            var damage = attackData.Power
                * attackerStatus.additionalStatus.AttackRate
                * (attackerStatus.attackBuffTimer > 0.0f ? attackerStatus.attackBuffRate : 1.0f)
                * (1f - additionalStatus.DamageCutRate)
                * (IsStunned ? defenseDebuffRateOnStunned : 1.0f);
            hitPoint.Value = Mathf.Max(0f, hitPoint.Value - damage);
            stunResistance.Value = Mathf.Min(stunResistance.Value + attackData.StunDamage, stunResistanceMax.Value);
            actorAnimation.SetBool(ActorAnimation.Parameter.IsAlive, hitPoint.Value > 0f);
            if (hitPoint.Value <= 0f)
            {
                actorAnimation.SetTrigger(ActorAnimation.Parameter.Dead);
                actor.Event.Broker.Publish(new ActorEvent.OnDead());
            }
            else if (stunResistance.Value >= stunResistanceMax.Value)
            {
                actorAnimation.PlayDamageAnimation(attackData.DamageId);
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
                BeginStunAsync().Forget();
                actor.Event.Broker.Publish(new ActorEvent.OnBeginStun());
            }

            actor.GetAbility<ActorTime>().Time.BeginHitStopAsync(attackData.HitStopDuration, attackData.HitStopTimeScale, actor.destroyCancellationToken).Forget();
            attacker.GetAbility<ActorTime>().Time.BeginHitStopAsync(attackData.HitStopDuration, attackData.HitStopTimeScale, attacker.destroyCancellationToken).Forget();

            LMotion.Shake.Create(attackData.SceneViewShakeStartValue, attackData.SceneViewShakeStrength, attackData.SceneViewShakeDuration)
                .WithFrequency(attackData.SceneViewShakeFrequency)
                .WithDampingRatio(attackData.SceneViewShakeDampingRatio)
                .BindToLocalPosition(actorSceneViewHandler.SceneView.transform)
                .AddTo(actor);

            return new TakeDamageData((int)damage);
        }

        public void RecoveryHitPoint(float rate)
        {
            if (hitPoint.Value <= 0f)
            {
                return;
            }
            var amount = hitPointMax.Value * rate;
            hitPoint.Value = Mathf.Min(hitPointMax.Value, hitPoint.Value + amount);
            actorAnimation.SetBool(ActorAnimation.Parameter.IsAlive, hitPoint.Value > 0f);
            actor.Event.Broker.Publish(new ActorEvent.OnRecoveryHitPoint());
        }

        public void SetAttackBuffTimer(float duration)
        {
            attackBuffTimer = duration;
            actor.Event.Broker.Publish(new ActorEvent.OnSetAttackBuffTimer());
        }

        public bool CanUseStamina()
        {
            return stamina.Value > 0f;
        }

        public void UseStamina(float amount)
        {
            stamina.Value -= amount;
        }

        public void AddSpecialPower(float amount)
        {
            var result = specialPower.Value + amount;
            if (specialStock.Value >= specialStockMax.Value)
            {
                specialPower.Value = Mathf.Min(result, 1.0f);
                return;
            }

            var addSpecialStock = (int)Mathf.Floor(result);
            if (addSpecialStock > 0)
            {
                specialStock.Value = Mathf.Min(specialStock.Value + addSpecialStock, specialStockMax.Value);
                result -= addSpecialStock;
                onSpecialStockReached?.InvokeAsync(actor, actor.destroyCancellationToken).Forget();
            }
            specialPower.Value = result;
        }

        public bool CanUseSpecialStock()
        {
            return specialStock.Value > 0;
        }

        public void UseSpecialStock()
        {
            specialStock.Value = Math.Max(0, specialStock.Value - 1);
        }

        public void BeginInvincible(string topic)
        {
            invincibleBlocker.Block(topic);
            isInvincible.Value = true;
        }

        public void EndInvincible(string topic)
        {
            invincibleBlocker.Unblock(topic);
            if (!invincibleBlocker.IsBlocked)
            {
                isInvincible.Value = false;
            }
        }

        private async UniTask BeginStunAsync()
        {
            if (IsStunned)
            {
                return;
            }
            onStunAction?.InvokeAsync(actor, actor.destroyCancellationToken).Forget();
            IsStunned = true;
            await UniTask.Delay(TimeSpan.FromSeconds(StunDuration), cancellationToken: actor.destroyCancellationToken);
            IsStunned = false;
            stunResistance.Value = 0f;
        }
    }
}
