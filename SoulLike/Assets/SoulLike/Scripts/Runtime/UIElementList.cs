using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoulLike
{
    public class UIElementList : MonoBehaviour
    {
        [field: SerializeField]
        public Button Button { get; private set; }

        [SerializeField]
        private Image icon;

        [SerializeField]
        private TMP_Text message;

        [SerializeField]
        private TMP_Text footer;

        public void Setup(Sprite icon, string message, string footer)
        {
            this.icon.sprite = icon;
            this.message.SetText(message);
            this.footer.SetText(footer);
        }
    }
}
