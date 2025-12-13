using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoulLike
{
    public class UIElementList : MonoBehaviour
    {
        [field: SerializeField]
        public Selectable Selectable { get; private set; }

        [SerializeField]
        private Image icon;

        [SerializeField]
        private TMP_Text message;

        public void Setup(Sprite icon, string message)
        {
            this.icon.sprite = icon;
            this.message.text = message;
        }
    }
}
