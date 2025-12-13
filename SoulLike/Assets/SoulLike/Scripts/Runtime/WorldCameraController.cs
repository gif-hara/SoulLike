using R3;
using SoulLike.ActorControllers;
using Unity.Cinemachine;
using UnityEngine;

namespace SoulLike
{
    public class WorldCameraController : MonoBehaviour
    {
        [field: SerializeField]
        public Camera WorldCamera { get; private set; }

        [field: SerializeField]
        private CinemachineCamera defaultCamera;

        [SerializeField]
        private CinemachineCamera lockOnCamera;

        [SerializeField]
        private CinemachineImpulseSource onGiveDamageImpulseSource;

        public void BeginObserve(Actor actor)
        {
            actor.Event.Broker.Receive<ActorEvent.OnGiveDamage>()
                .Subscribe(this, (x, @this) =>
                {
                    if (!x.IsStunned)
                    {
                        return;
                    }
                    @this.onGiveDamageImpulseSource.GenerateImpulse();
                })
                .RegisterTo(actor.destroyCancellationToken);
        }

        public void SetDefaultCameraTarget(Transform target)
        {
            defaultCamera.Target.TrackingTarget = target;
        }

        public void SetLockOnCameraTarget(Transform follow, Transform lookAt)
        {
            lockOnCamera.Target.TrackingTarget = follow;
            lockOnCamera.Target.LookAtTarget = lookAt;
        }

        public void SetActiveLockOnCamera(bool isActive)
        {
            lockOnCamera.gameObject.SetActive(isActive);
        }
    }
}
