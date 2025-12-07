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

        public void EnableAttack(string topic)
        {
            actor.GetAbility<ActorWeaponHandler>().AttackBlocker.Unblock(topic);
        }

        public void BlockMove(string topic)
        {
            actor.GetAbility<ActorMovement>().MoveBlocker.Block(topic);
        }

        public void UnblockMove(string topic)
        {
            actor.GetAbility<ActorMovement>().MoveBlocker.Unblock(topic);
        }

        public void BlockRotate(string topic)
        {
            actor.GetAbility<ActorMovement>().RotateBlocker.Block(topic);
        }

        public void UnblockRotate(string topic)
        {
            actor.GetAbility<ActorMovement>().RotateBlocker.Unblock(topic);
        }

        public void BeginInvincible(string topic)
        {
            actor.GetAbility<ActorInvincible>().BeginInvincible(topic);
        }

        public void EndInvincible(string topic)
        {
            actor.GetAbility<ActorInvincible>().EndInvincible(topic);
        }
    }
}
