using System;
using System.Threading;
using SoulLike.ActorControllers;
using SoulLike.ActorControllers.Abilities;

namespace SoulLike.WeaponActions
{
    [Serializable]
    public sealed class EndParry : IWeaponAction
    {
        public void Invoke(Weapon weapon, Actor actor, CancellationToken scope)
        {
            actor.GetAbility<ActorStatus>().IsParrying = false;
        }
    }
}
