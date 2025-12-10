using System;
using System.Threading;
using SoulLike.ActorControllers;
using UnityEngine;

namespace SoulLike.WeaponActions
{
    [Serializable]
    public sealed class ForceInvokeUniqueAttack : IWeaponAction
    {
        [SerializeField]
        private int attackId;

        public void Invoke(Weapon weapon, Actor actor, CancellationToken scope)
        {
            weapon.ForceInvokeUniqueAttack(attackId);
        }
    }
}
