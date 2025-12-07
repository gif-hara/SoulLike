using UnityEngine;

namespace SoulLike.ActorControllers.AISystems.SelectSequences
{
    public interface ISelectSequence
    {
        string Select(Actor actor, ActorAIController actorAIController);
    }
}
