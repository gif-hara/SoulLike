using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using R3;
using TMPro;
using UnityEngine;

namespace SoulLike
{
    public class UIViewDialog : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text message;

        [SerializeField]
        private Transform buttonParent;

        [SerializeField]
        private UIElementButton uiElementButtonPrefab;

        private List<UIElementButton> buttonInstances = new();

        public void Initialize()
        {
            gameObject.SetActive(false);
        }

        public async UniTask<int> ShowAsync(string message, string[] options)
        {
            this.message.SetText(message);
            for (int i = 0; i < options.Length; i++)
            {
                var buttonInstance = Instantiate(uiElementButtonPrefab, buttonParent);
                buttonInstance.Text.SetText(options[i]);
                buttonInstances.Add(buttonInstance);
            }
            buttonInstances.Select(x => x.Button).ToList().SetNavigationHorizontal();
            buttonInstances[0].Button.Select();
            gameObject.SetActive(true);
            var result = await UniTask.WhenAny(buttonInstances.Select(x => x.Button.OnClickAsync()));
            gameObject.SetActive(false);
            foreach (var buttonInstance in buttonInstances)
            {
                Destroy(buttonInstance.gameObject);
            }
            buttonInstances.Clear();

            return result;
        }
    }
}
