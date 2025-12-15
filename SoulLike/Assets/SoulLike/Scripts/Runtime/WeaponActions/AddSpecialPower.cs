using System;
using System.Threading;
using HK;
using SoulLike.ActorControllers;
using SoulLike.ActorControllers.Abilities;
using UnityEngine;

namespace SoulLike.WeaponActions
{
    [Serializable]
    public sealed class AddSpecialPower : IWeaponAction
    {
        [SerializeField]
        private float amount;

        public void Invoke(Weapon weapon, Actor actor, CancellationToken scope)
        {
            actor.GetAbility<ActorStatus>().AddSpecialPower(amount);
        }
    }
}
