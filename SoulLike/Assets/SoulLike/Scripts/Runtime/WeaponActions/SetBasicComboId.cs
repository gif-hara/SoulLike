using System;
using System.Threading;
using SoulLike.ActorControllers;
using UnityEngine;

namespace SoulLike.WeaponActions
{
    [Serializable]
    public sealed class SetBasicComboId : IWeaponAction
    {
        [SerializeField]
        private int comboId;

        public void Invoke(Weapon weapon, Actor actor, CancellationToken scope)
        {
            weapon.BasicAttackComboId = comboId;
        }
    }
}
