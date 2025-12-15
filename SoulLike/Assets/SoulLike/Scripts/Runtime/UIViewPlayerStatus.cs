using System.Collections.Generic;
using R3;
using SoulLike.ActorControllers;
using SoulLike.ActorControllers.Abilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoulLike
{
    public class UIViewPlayerStatus : MonoBehaviour
    {
        [SerializeField]
        private Slider hitPointSlider;

        [SerializeField]
        private Slider staminaSlider;

        [SerializeField]
        private TMP_Text experienceText;

        [SerializeField]
        private Slider specialPowerSlider;

        [SerializeField]
        private Transform specialStockParent;

        [SerializeField]
        private UIViewSpecialStockElement specialStockElementPrefab;

        [SerializeField]
        private float hitPointSliderLengthRate;

        [SerializeField]
        private float staminaSliderLengthRate;

        private List<UIViewSpecialStockElement> specialStockElements = new();

        public void Bind(Actor actor, UserData userData)
        {
            var actorStatus = actor.GetAbility<ActorStatus>();
            actorStatus.HitPointMax
                .Subscribe(this, static (x, @this) =>
                {
                    var t = (RectTransform)@this.hitPointSlider.transform;
                    var sizeDelta = t.sizeDelta;
                    t.sizeDelta = new Vector2(x * @this.hitPointSliderLengthRate, sizeDelta.y);
                })
                .RegisterTo(actor.destroyCancellationToken);
            actorStatus.StaminaMax
                .Subscribe(this, static (x, @this) =>
                {
                    var t = (RectTransform)@this.staminaSlider.transform;
                    var sizeDelta = t.sizeDelta;
                    t.sizeDelta = new Vector2(x * @this.staminaSliderLengthRate, sizeDelta.y);
                })
                .RegisterTo(actor.destroyCancellationToken);
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
                actorStatus.StaminaMax,
                actorStatus.Stamina
            )
                .Subscribe((this, actorStatus), static (_, t) =>
                {
                    var (@this, actorStatus) = t;
                    @this.staminaSlider.value = actorStatus.StaminaRate;
                })
                .RegisterTo(actor.destroyCancellationToken);
            userData.Experience
                .Subscribe(this, static (x, @this) =>
                {
                    @this.experienceText.text = x.ToString();
                })
                .RegisterTo(actor.destroyCancellationToken);
            actorStatus.SpecialPower
                .Subscribe((this, actorStatus), static (x, t) =>
                {
                    var (@this, actorStatus) = t;
                    @this.specialPowerSlider.value = x;
                })
                .RegisterTo(actor.destroyCancellationToken);
            actorStatus.SpecialStock
                .Subscribe((this, actorStatus), static (x, t) =>
                {
                    var (@this, actorStatus) = t;
                    for (var i = 0; i < @this.specialStockElements.Count; i++)
                    {
                        @this.specialStockElements[i].SetActive(i < x);
                    }
                })
                .RegisterTo(actor.destroyCancellationToken);
            actorStatus.SpecialStockMax
                .Subscribe((this, actorStatus), static (_, t) =>
                {
                    var (@this, actorStatus) = t;
                    var diff = actorStatus.SpecialStockMax.CurrentValue - @this.specialStockElements.Count;
                    if (diff > 0)
                    {
                        for (var i = 0; i < diff; i++)
                        {
                            var element = Instantiate(@this.specialStockElementPrefab, @this.specialStockParent);
                            element.SetActive(actorStatus.SpecialStock.CurrentValue > @this.specialStockElements.Count);
                            @this.specialStockElements.Add(element);
                        }
                    }
                    else if (diff < 0)
                    {
                        for (var i = 0; i < -diff; i++)
                        {
                            var lastIndex = @this.specialStockElements.Count - 1;
                            Destroy(@this.specialStockElements[lastIndex].gameObject);
                            @this.specialStockElements.RemoveAt(lastIndex);
                        }
                    }
                })
                .RegisterTo(actor.destroyCancellationToken);
        }
    }
}
