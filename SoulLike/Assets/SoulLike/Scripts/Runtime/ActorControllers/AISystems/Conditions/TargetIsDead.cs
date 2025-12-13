using System;
using SoulLike.ActorControllers.Abilities;

namespace SoulLike.ActorControllers.AISystems.Conditions
{
    [Serializable]
    public sealed class TargetIsDead : IAICondition
    {
        public bool Evaluate(Actor actor, ActorAIController actorAIController)
        {
            return actor.GetAbility<ActorTargetHandler>().Target.GetAbility<ActorStatus>().IsDead;
        }
    }
}
