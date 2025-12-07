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
        }

        public void TakeDamage(Actor attacker, AttackData attackData)
        {
            var lookDirection = attacker.transform.position - actor.transform.position;
            lookDirection.y = 0f;
            actorMovement.RotateImmediate(Quaternion.LookRotation(lookDirection.normalized));
            actorAnimation.PlayDamageAnimation(attackData.DamageId);
        }
    }
}
