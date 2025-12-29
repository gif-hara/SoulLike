using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
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
            return startButton.OnClickAsync(cancellationToken);
        }
    }
}
