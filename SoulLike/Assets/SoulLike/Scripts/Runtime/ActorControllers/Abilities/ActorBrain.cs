using System.Threading;
using SoulLike.ActorControllers.Brains;
using UnityEngine;

namespace SoulLike.ActorControllers.Abilities
{
    public sealed class ActorBrain : IActorAbility
    {
        private Actor actor;

        private CancellationTokenSource scope;

        public void Activate(Actor actor)
        {
            this.actor = actor;
        }

        public void Attach(IActorBrain brain)
        {
            scope = CancellationTokenSource.CreateLinkedTokenSource(actor.destroyCancellationToken, Application.exitCancellationToken);
            brain.Attach(actor, scope.Token);
        }
    }
}
