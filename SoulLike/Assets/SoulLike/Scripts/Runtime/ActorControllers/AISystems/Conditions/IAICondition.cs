using UnityEngine;

namespace SoulLike.ActorControllers.AISystems.Conditions
{
    public interface IAICondition
    {
        bool Evaluate(Actor actor, ActorAIController actorAIController);
    }
}
