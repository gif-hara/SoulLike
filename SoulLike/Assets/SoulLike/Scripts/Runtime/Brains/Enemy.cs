using System.Threading;
using HK;

namespace SoulLike.ActorControllers.Brains
{
    public sealed class Enemy : IActorBrain
    {
        public Enemy()
        {
        }

        public void Attach(Actor actor, CancellationToken cancellationToken)
        {
            actor.gameObject.SetLayerRecursive(Layer.Enemy);
        }
    }
}
