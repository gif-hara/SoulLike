using System;
using System.Threading;
using HK;
using SoulLike.ActorControllers;
using UnityEngine;

namespace SoulLike.WeaponActions
{
    [Serializable]
    public sealed class PlaySfx : IWeaponAction
    {
        [SerializeField]
        private string sfxKey;

        public void Invoke(Weapon weapon, Actor actor, CancellationToken scope)
        {
            TinyServiceLocator.Resolve<AudioManager>().PlaySfx(sfxKey);
        }
    }
}
