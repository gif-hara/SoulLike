using System.Threading;

namespace SoulLike.ActorControllers.Abilities
{
    public interface IActorAbility
    {
        void Activate(Actor actor, CancellationToken cancellationToken);
    }
}
