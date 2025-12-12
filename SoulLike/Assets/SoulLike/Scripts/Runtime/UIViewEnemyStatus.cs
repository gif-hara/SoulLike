using System;
using LitMotion;
using R3;
using SoulLike.ActorControllers;
using SoulLike.ActorControllers.Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace SoulLike
{
    public class UIViewEnemyStatus : MonoBehaviour
    {
        [SerializeField]
        private Slider hitPointSlider;

        [SerializeField]
        private Slider stunSlider;

        private MotionHandle stunnedSliderUpdate = MotionHandle.None;

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
            Observable.Merge(
                actorStatus.StunResistanceMax,
                actorStatus.StunResistance
            )
                .Subscribe((this, actorStatus), static (_, t) =>
                {
                    var (@this, actorStatus) = t;
                    if (actorStatus.StunResistanceRate >= 1.0f)
                    {
                        if (@this.stunnedSliderUpdate == MotionHandle.None)
                        {
                            @this.stunnedSliderUpdate = LMotion.Create(1.0f, 0.0f, actorStatus.StunDuration)
                                .WithOnComplete(() =>
                                {
                                    @this.stunnedSliderUpdate = MotionHandle.None;
                                })
                                .Bind(x => @this.stunSlider.value = x)
                                .AddTo(@this);
                        }
                    }
                    else
                    {
                        @this.stunSlider.value = actorStatus.StunResistanceRate;
                    }
                })
                .RegisterTo(actor.destroyCancellationToken);
        }
    }
}
