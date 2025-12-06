using UnityEngine;

namespace SoulLike.ActorControllers.Abilities
{
    public class ActorStatus : IActorAbility
    {
        private Actor actor;

        private ActorMovement actorMovement;

        private ActorAnimation actorAnimation;

        public void Activate(Actor actor)
        {
            this.actor = actor;
            actorMovement = actor.GetAbility<ActorMovement>();
            actorAnimation = actor.GetAbility<ActorAnimation>();
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
