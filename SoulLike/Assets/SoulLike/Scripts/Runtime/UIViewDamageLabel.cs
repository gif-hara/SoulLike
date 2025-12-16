using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using R3;
using SoulLike.ActorControllers;
using UnityEngine;
using UnityEngine.UI;

namespace SoulLike
{
    public class UIViewDamageLabel : MonoBehaviour
    {
        [SerializeField]
        private RectTransform labelParent;

        [SerializeField]
        private UIElementDamageLabel damageLabelPrefab;

        public void BeginObserve(Actor actor, Camera worldCamera)
        {
            actor.Event.Broker.Receive<ActorEvent.OnGiveDamage>()
                .Subscribe((this, actor, worldCamera), static (x, t) =>
                {
                    var (@this, actor, worldCamera) = t;
                    var damageLabel = Instantiate(@this.damageLabelPrefab, @this.labelParent);
                    damageLabel.Setup(x.Damage.ToString(), x.hitPoint, worldCamera);
                })
                .RegisterTo(destroyCancellationToken);
        }
    }
}
