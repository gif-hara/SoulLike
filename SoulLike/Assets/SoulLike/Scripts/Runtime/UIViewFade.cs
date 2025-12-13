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

        public async UniTask BeginAsync(Color color, float duration, CancellationToken cancellationToken)
        {
            await LMotion.Create(image.color, color, duration)
                .BindToColor(image)
                .ToUniTask(cancellationToken);
        }
    }
}
