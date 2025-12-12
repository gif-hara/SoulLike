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

        [SerializeField]
        private Volume onGiveDamageInStunVolume;

        [SerializeField]
        private float onGiveDamageInStunEffectDuration;

        [SerializeField]
        private Ease onGiveDamageInStunEffectEase;

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
                .RegisterTo(actor.destroyCancellationToken);
            actor.Event.Broker.Receive<ActorEvent.OnGiveDamage>()
                .Subscribe(this, static (e, @this) =>
                {
                    if (!e.IsStunned)
                    {
                        return;
                    }

                    LMotion.Create(1.0f, 0.0f, @this.onGiveDamageInStunEffectDuration)
                        .WithEase(@this.onGiveDamageInStunEffectEase)
                        .Bind(x => @this.onGiveDamageInStunVolume.weight = x)
                        .AddTo(@this);
                })
                .RegisterTo(actor.destroyCancellationToken);
        }
    }
}
