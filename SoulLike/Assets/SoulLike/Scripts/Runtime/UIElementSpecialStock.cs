using UnityEngine;

namespace SoulLike
{
    public class UIViewSpecialStockElement : MonoBehaviour
    {
        [SerializeField]
        private GameObject activeObject;

        [SerializeField]
        private GameObject inactiveObject;

        public void SetActive(bool isActive)
        {
            activeObject.SetActive(isActive);
            inactiveObject.SetActive(!isActive);
        }
    }
}
