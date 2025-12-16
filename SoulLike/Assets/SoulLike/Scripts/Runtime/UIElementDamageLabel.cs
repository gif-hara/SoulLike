using TMPro;
using UnityEngine;

namespace SoulLike
{
    public class UIElementDamageLabel : MonoBehaviour
    {
        [SerializeField]
        private RectTransform labelRoot;

        [SerializeField]
        private TMP_Text label;

        public void Setup(string text, Vector3 worldPosition, Camera worldCamera)
        {
            label.text = text;
            var screenPosition = worldCamera.WorldToScreenPoint(worldPosition);
            labelRoot.position = screenPosition;
        }
    }
}
