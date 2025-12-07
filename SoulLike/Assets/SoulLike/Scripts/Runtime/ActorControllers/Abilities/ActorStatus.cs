using R3;
using UnityEngine;

namespace SoulLike.ActorControllers.Abilities
{
    public class ActorStatus : IActorAbility
    {
        private Actor actor;

        private ActorMovement actorMovement;

        private ActorAnimation actorAnimation;

        private ReactiveProperty<float> hitPoint = new();

        private ReactiveProperty<float> hitPointMax = new();

        public ReadOnlyReactiveProperty<float> HitPoint => hitPoint;

        public ReadOnlyReactiveProperty<float> HitPointMax => hitPointMax;

        public void Activate(Actor actor)
        {
            this.actor = actor;
            actorMovement = actor.GetAbility<ActorMovement>();
            actorAnimation = actor.GetAbility<ActorAnimation>();
        }

        public void ApplySpec(MasterDataSystem.ActorStatusSpec spec)
        {
            hitPointMax.Value = spec.HitPoint;
            hitPoint.Value = spec.HitPoint;
            actorAnimation.SetBool(ActorAnimation.Parameter.IsAlive, hitPoint.Value > 0f);
        }

        public void TakeDamage(Actor attacker, AttackData attackData)
        {
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
        }
    }
}
