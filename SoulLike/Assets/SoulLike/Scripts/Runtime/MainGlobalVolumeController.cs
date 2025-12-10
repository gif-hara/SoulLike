using LitMotion;
using R3;
using SoulLike.ActorControllers;
using UnityEngine;
using UnityEngine.Rendering;

namespace SoulLike
{
    public class MainGlobalVolumeController : MonoBehaviour
    {
        [SerializeField]
        private Volume parryVolume;

        [SerializeField]
        private float parryEffectDuration;

        [SerializeField]
        private Ease parryEffectEase;


        public void BeginObserve(Actor actor)
        {
            actor.Event.Broker.Receive<ActorEvent.OnBeginParry>()
                .Subscribe(this, static (_, @this) =>
                {
                    LMotion.Create(1.0f, 0.0f, @this.parryEffectDuration)
                        .WithEase(@this.parryEffectEase)
                        .Bind(x => @this.parryVolume.weight = x)
                        .AddTo(@this);
                })
                .AddTo(actor);
        }
    }
}
