using HK;
using SoulLike.ActorControllers.Abilities;
using UnityEngine;

namespace SoulLike.ActorControllers
{
    public class ActorAnimationEvent : MonoBehaviour
    {
        private Actor actor;

        public void Activate(Actor actor)
        {
            this.actor = actor;
        }

        public void SetRotateImmediateTargetRotation()
        {
            actor.GetAbility<ActorMovement>().RotateImmediate(actor.GetAbility<ActorMovement>().TargetRotation);
        }

        public void PlaySfx(string key)
        {
            TinyServiceLocator.Resolve<AudioManager>().PlaySfx(key);
        }

        public void BeginAttack(int attackId)
        {
            actor.Event.Broker.Publish(new ActorEvent.BeginAttack(attackId));
        }

        public void EndAttack(int attackId)
        {
            actor.Event.Broker.Publish(new ActorEvent.EndAttack(attackId));
        }

        public void EnableAttack()
        {
            actor.GetAbility<ActorWeaponHandler>().CanAttack.Value = true;
        }
    }
}
