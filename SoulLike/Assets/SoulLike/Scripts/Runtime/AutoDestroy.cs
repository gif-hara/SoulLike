using UnityEngine;

namespace SoulLike
{
    public class AutoDestroy : MonoBehaviour
    {
        [SerializeField]
        private float delay = 1f;

        void Start()
        {
            Destroy(gameObject, delay);
        }
    }
}
