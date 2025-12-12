using System;
using System.Threading;
using Cysharp.Threading.Tasks;
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

        [SerializeField]
        private Image stunSliderImage;

        [SerializeField]
        private Color stunDefaultColor;

        [SerializeField]
        private Color stunnedColor;

        [SerializeField]
        private float stunnedColorAnimationDuration;

        [SerializeField]
        private Ease stunnedColorAnimationEase;

        private MotionHandle stunnedSliderUpdate = MotionHandle.None;

        private bool playingStunnedColorAnimation = false;

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
                        if (!@this.playingStunnedColorAnimation)
                        {
                            @this.BeginStunnedGaugeAnimationAsync(actorStatus.StunDuration, @this.destroyCancellationToken).Forget();
                        }
                    }
                    else
                    {
                        @this.stunSlider.value = actorStatus.StunResistanceRate;
                    }
                })
                .RegisterTo(actor.destroyCancellationToken);
        }

        private async UniTask BeginStunnedGaugeAnimationAsync(float duration, CancellationToken cancellationToken)
        {
            var colorAnimation = LMotion.Create(stunnedColor, stunDefaultColor, stunnedColorAnimationDuration)
                .WithEase(stunnedColorAnimationEase)
                .WithLoops(-1, LoopType.Yoyo)
                .Bind(x => stunSliderImage.color = x)
                .AddTo(this);
            await LMotion.Create(1.0f, 0.0f, duration)
                .Bind(x => stunSlider.value = x)
                .AddTo(this)
                .ToUniTask(cancellationToken);
            colorAnimation.Cancel();
            stunSliderImage.color = stunDefaultColor;
        }
    }
}
