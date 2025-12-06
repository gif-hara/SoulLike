using UnityEngine;

namespace SoulLike.ActorControllers.Abilities
{
    public class ActorStatus : IActorAbility
    {
        private Actor actor;

        private ActorAnimation actorAnimation;

        public void Activate(Actor actor)
        {
            this.actor = actor;
            actorAnimation = actor.GetAbility<ActorAnimation>();
        }

        public void TakeDamage(AttackData attackData)
        {
            actorAnimation.PlayDamageAnimation(attackData.DamageId);
        }
    }
}
