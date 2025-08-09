using HK;
using SoulLike.ActorControllers;
using SoulLike.ActorControllers.Abilities;
using UnityEngine;

namespace SoulLike
{
    public class ActorEventMediator : MonoBehaviour
    {
        [SerializeField]
        private Actor actor;

        public void SetCanRotate(int value)
        {
            actor.FindAbility<ActorMovementController>().CanRotate.Value = value == 1;
        }

        public void SetRotateImmediateTargetRotation()
        {
            actor.FindAbility<ActorMovementController>().RotateImmediate(actor.FindAbility<ActorMovementController>().TargetRotation);
        }

        public void PlaySfx(string key)
        {
            TinyServiceLocator.Resolve<AudioManager>().PlaySfx(key);
        }
    }
}
