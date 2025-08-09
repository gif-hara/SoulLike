using System.Threading;
using UnityEngine;

namespace SoulLike.ActorControllers.Brains
{
    public sealed class Player : IActorBrain
    {
        public void Attach(Actor actor, CancellationToken cancellationToken)
        {
            Debug.Log("Player brain attached to actor: " + actor.name);
        }
    }
}
