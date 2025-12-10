using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SoulLike.ActorControllers;
using SoulLike.ActorControllers.Abilities;
using UnityEngine;

namespace SoulLike.WeaponActions
{
    [Serializable]
    public sealed class BeginHitStop : IWeaponAction
    {
        [SerializeField]
        private float duration = 0.1f;

        [SerializeField]
        private float timeScale = 0.0f;

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
            target.GetAbility<ActorTime>().Time.BeginHitStopAsync(duration, timeScale, default).Forget();
        }
    }
}
