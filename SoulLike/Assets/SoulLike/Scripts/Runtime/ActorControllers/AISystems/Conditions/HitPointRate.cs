using System;
using SoulLike.ActorControllers.Abilities;
using UnityEngine;

namespace SoulLike.ActorControllers.AISystems.Conditions
{
    [Serializable]
    public sealed class HitPointRate : IAICondition
    {
        public enum ComparisonType
        {
            GreaterThan,
            GreaterThanOrEqual,
            LessThan,
            LessThanOrEqual,
        }

        [SerializeField]
        private ComparisonType comparisonType;

        [SerializeField]
        private float threshold;

        public bool Evaluate(Actor actor, ActorAIController actorAIController)
        {
            float hitPointRate = (float)actor.GetAbility<ActorStatus>().HitPointRate;

            return comparisonType switch
            {
                ComparisonType.GreaterThan => hitPointRate > threshold,
                ComparisonType.GreaterThanOrEqual => hitPointRate >= threshold,
                ComparisonType.LessThan => hitPointRate < threshold,
                ComparisonType.LessThanOrEqual => hitPointRate <= threshold,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}
