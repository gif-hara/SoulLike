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

        public void SetDefaultCameraTarget(Transform target)
        {
            defaultCamera.Target.TrackingTarget = target;
        }
    }
}
