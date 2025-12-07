using R3;
using SoulLike.ActorControllers;
using SoulLike.ActorControllers.Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace SoulLike
{
    public class UIViewPlayerStatus : MonoBehaviour
    {
        [SerializeField]
        private Slider hitPointSlider;

        public void Bind(Actor actor)
        {
            var actorStatus = actor.GetAbility<ActorStatus>();
            Observable.Merge(
                actorStatus.HitPointMax,
                actorStatus.HitPoint
            )
                .Subscribe((this, actorStatus), static (_, t) =>
                {
                    var (@this, actorStatus) = t;
                    @this.hitPointSlider.value = actorStatus.HitPointRate;
                })
                .RegisterTo(actor.destroyCancellationToken);
        }
    }
}
