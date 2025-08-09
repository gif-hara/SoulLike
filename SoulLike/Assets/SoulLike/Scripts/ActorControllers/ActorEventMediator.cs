using HK;
using MH3.ActorControllers;
using UnityEngine;

namespace MH3
{
    public class ActorEventMediator : MonoBehaviour
    {
        [SerializeField]
        private Actor actor;

        public void SetCanRotate(int value)
        {
            actor.MovementController.CanRotate.Value = value == 1;
        }

        public void SetRotateImmediateTargetRotation()
        {
            actor.MovementController.RotateImmediate(actor.MovementController.TargetRotation);
        }

        public void PlaySfx(string key)
        {
            TinyServiceLocator.Resolve<AudioManager>().PlaySfx(key);
        }
    }
}
