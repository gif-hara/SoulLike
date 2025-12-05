using System;
using System.Threading;
using SoulLike.ActorControllers;
using UnityEngine;

namespace SoulLike.WeaponActions
{
    [Serializable]
    public sealed class SetActiveGameObject : IWeaponAction
    {
        [SerializeField]
        private GameObject target;

        [SerializeField]
        private bool isActive;

        public void Invoke(Weapon weapon, Actor actor, CancellationToken scope)
        {
            target.SetActive(isActive);
        }
    }
}
