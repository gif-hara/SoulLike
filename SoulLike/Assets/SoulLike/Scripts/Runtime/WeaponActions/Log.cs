using System;
using System.Threading;
using SoulLike.ActorControllers;
using UnityEngine;

namespace SoulLike.WeaponActions
{
    [Serializable]
    public sealed class Log : IWeaponAction
    {
        [SerializeField]
        private string message;

        public void Invoke(Weapon weapon, Actor actor, CancellationToken scope)
        {
            Debug.Log(message);
        }
    }
}
