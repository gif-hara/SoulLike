using System.Threading;
using SoulLike.ActorControllers.Brains;
using UnityEngine;

namespace SoulLike.ActorControllers.Abilities
{
    public sealed class ActorAttack : IActorAbility
    {
        private ActorAnimation actorAnimation;

        public void Activate(Actor actor)
        {
            actorAnimation = actor.GetAbility<ActorAnimation>();
        }

        public bool TryAttack()
        {
            actorAnimation.SetTrigger(ActorAnimation.Parameter.Attack);
            return true;
        }
    }
}
