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
            actor.GetAbility<ActorStatus>().BeginInvincible(topic);
        }

        public void EndInvincible(string topic)
        {
            actor.GetAbility<ActorStatus>().EndInvincible(topic);
        }

        public void BeginParry()
        {
            actor.GetAbility<ActorStatus>().IsParrying = true;
        }

        public void EndParry()
        {
            actor.GetAbility<ActorStatus>().IsParrying = false;
        }

        public void EnableDodge(string topic)
        {
            actor.GetAbility<ActorDodge>().DodgeBlocker.Unblock(topic);
        }

        public void RecoveryHitPoint(float rate)
        {
            actor.GetAbility<ActorStatus>().RecoveryHitPoint(rate);
        }

        public void SetAttackBuffTimer(float duration)
        {
            actor.GetAbility<ActorStatus>().SetAttackBuffTimer(duration);
        }

        public void PlayEffect(string key)
        {
            actor.GetAbility<ActorEffect>().SetActive(key, true);
        }

        public void PlayCameraImpulse(int type)
        {
            var worldCameraController = TinyServiceLocator.Resolve<WorldCameraController>();
            worldCameraController.PlayImpulse((WorldCameraController.ImpluseType)type);
        }
    }
}
