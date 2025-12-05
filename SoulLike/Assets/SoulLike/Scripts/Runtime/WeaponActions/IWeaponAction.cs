using System.Threading;
using SoulLike.ActorControllers;

namespace SoulLike.WeaponActions
{
    public interface IWeaponAction
    {
        void Invoke(Weapon weapon, Actor actor, CancellationToken scope);
    }
}
