using System.Threading;
using HK;
using SoulLike.ActorControllers.Abilities;

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
            actor.AddAbility<ActorTime>();
            actor.AddAbility<ActorMovement>();
            actor.AddAbility<ActorSceneViewHandler>();
            actor.AddAbility<ActorAnimation>();
            actor.AddAbility<ActorStatus>();
            actor.AddAbility<ActorTargetHandler>();
        }
    }
}
