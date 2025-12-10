using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using SoulLike.ActorControllers;
using SoulLike.ActorControllers.Abilities;
using UnityEngine;

namespace SoulLike.WeaponActions
{
    [Serializable]
    public sealed class SceneViewShake : IWeaponAction
    {
        [SerializeField]
        private Vector3 startValue;

        [SerializeField]
        private Vector3 strength;

        [SerializeField]
        private int frequency = 10;

        [SerializeField]
        private int dampingRatio = 1;

        [SerializeField]
        private float duration;

        [SerializeField]
        private TragetType targetType;

        public enum TragetType
        {
            Self,
            Target
        }

        public void Invoke(Weapon weapon, Actor actor, CancellationToken scope)
        {
            var target = targetType == TragetType.Self ? actor : actor.GetAbility<ActorTargetHandler>().Target;
            LMotion.Shake.Create(startValue, strength, duration)
                .WithFrequency(frequency)
                .WithDampingRatio(dampingRatio)
                .BindToLocalPosition(target.GetAbility<ActorSceneViewHandler>().SceneView.transform)
                .AddTo(actor);
        }
    }
}
