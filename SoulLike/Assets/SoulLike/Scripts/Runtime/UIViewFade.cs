using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace SoulLike
{
    public class UIViewFade : MonoBehaviour
    {
        [SerializeField]
        private Image image;

        public UniTask BeginAsync(Color to, float duration, CancellationToken cancellationToken)
        {
            return BeginAsync(image.color, to, duration, cancellationToken);
        }

        public UniTask BeginAsync(Color from, Color to, float duration, CancellationToken cancellationToken)
        {
            return LMotion.Create(from, to, duration)
                .BindToColor(image)
                .ToUniTask(cancellationToken);
        }
    }
}
