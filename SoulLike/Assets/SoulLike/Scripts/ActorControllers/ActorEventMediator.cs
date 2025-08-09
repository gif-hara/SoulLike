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
            actor.FindAbility<ActorMovement>().CanRotate.Value = value == 1;
        }

        public void SetRotateImmediateTargetRotation()
        {
            actor.FindAbility<ActorMovement>().RotateImmediate(actor.FindAbility<ActorMovement>().TargetRotation);
        }

        public void PlaySfx(string key)
        {
            TinyServiceLocator.Resolve<AudioManager>().PlaySfx(key);
        }
    }
}
