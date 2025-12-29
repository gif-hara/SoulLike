using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoulLike
{
    public class UIViewTitle : MonoBehaviour
    {
        [SerializeField]
        private Button startButton;

        [SerializeField]
        private Slider bgmSlider;

        [SerializeField]
        private Slider sfxSlider;

        public UniTask BeginAsync(CancellationToken cancellationToken)
        {
            EventSystem.current.SetSelectedGameObject(startButton.gameObject);
            return startButton.OnClickAsync(cancellationToken);
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}
