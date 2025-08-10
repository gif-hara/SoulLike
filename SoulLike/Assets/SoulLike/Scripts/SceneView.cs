using UnityEngine;

namespace SoulLike
{
    public class SceneView : MonoBehaviour
    {
        [field: SerializeField]
        public LocatorHolder LocatorHolder { get; private set; }

        [field: SerializeField]
        public Animator Animator { get; private set; }
    }
}
