using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
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

        [SerializeField]
        private string stunMaterialColorPropertyName = "_Color";

        [SerializeField]
        private CanvasGroup rootCanvasGroup;

        private bool playingStunnedColorAnimation = false;

        private Material stunMaterial;

        public void Bind(Actor actor)
        {
            var actorStatus = actor.GetAbility<ActorStatus>();
            stunMaterial = Instantiate(stunSliderImage.material);
            stunSliderImage.material = stunMaterial;
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
            actor.Event.Broker.Receive<ActorEvent.OnDead>()
                .Subscribe(this, static (_, @this) =>
                {
                    @this.BeginDeadAnimationAsync(@this.destroyCancellationToken).Forget();
                })
                .RegisterTo(actor.destroyCancellationToken);
            actor.Event.Broker.Receive<ActorEvent.ReviveEnemy>()
                .Subscribe(this, static (_, @this) =>
                {
                    @this.BeginReviveAnimationAsync(@this.destroyCancellationToken).Forget();
                })
                .RegisterTo(actor.destroyCancellationToken);
        }

        private async UniTask BeginStunnedGaugeAnimationAsync(float duration, CancellationToken cancellationToken)
        {
            var colorAnimation = LMotion.Create(stunnedColor, stunDefaultColor, stunnedColorAnimationDuration)
                .WithEase(stunnedColorAnimationEase)
                .WithLoops(-1, LoopType.Yoyo)
                .BindToMaterialColor(stunMaterial, stunMaterialColorPropertyName)
                .AddTo(this);
            await LMotion.Create(1.0f, 0.0f, duration)
                .Bind(x => stunSlider.value = x)
                .AddTo(this)
                .ToUniTask(cancellationToken);
            colorAnimation.Cancel();
            stunMaterial.SetColor(stunMaterialColorPropertyName, stunDefaultColor);
        }

        private async UniTask BeginDeadAnimationAsync(CancellationToken cancellationToken)
        {
            await LMotion.Create(1.0f, 0.0f, 1.0f)
                .WithDelay(1.0f)
                .BindToAlpha(rootCanvasGroup)
                .AddTo(this)
                .ToUniTask(cancellationToken);
        }

        private async UniTask BeginReviveAnimationAsync(CancellationToken cancellationToken)
        {
            await LMotion.Create(0.0f, 1.0f, 1.0f)
                .BindToAlpha(rootCanvasGroup)
                .AddTo(this)
                .ToUniTask(cancellationToken);
        }
    }
}
