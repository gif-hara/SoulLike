using System.Threading;

namespace SoulLike.ActorControllers.Brains
{
    public interface IActorBrain
    {
        void Attach(Actor actor, CancellationToken cancellationToken);
    }
}
