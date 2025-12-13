using SoulLike.ActorControllers.AISystems.Conditions;
using TNRD;
using UnityEngine;

namespace SoulLike.ActorControllers.AISystems.SelectSequences
{
    public class Condition : ISelectSequence
    {
        [SerializeField]
        private SerializableInterface<IAICondition> condition;

        [SerializeField]
        private string sequenceName;

        public string Select(Actor actor, ActorAIController actorAIController)
        {
            if (condition.Value.Evaluate(actor, actorAIController))
            {
                return sequenceName;
            }

            return string.Empty;
        }
    }
}
