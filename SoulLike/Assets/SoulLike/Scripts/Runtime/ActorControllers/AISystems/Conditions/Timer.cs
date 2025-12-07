using System;
using UnityEngine;

namespace SoulLike.ActorControllers.AISystems.Conditions
{
    [Serializable]
    public sealed class Timer : IAICondition
    {
        [SerializeField, Min(0f)]
        private float durationMin;

        [SerializeField, Min(0f)]
        private float durationMax;

        private float duration;

        public bool Evaluate(Actor actor, ActorAIController actorAIController)
        {
            if (duration <= 0f)
            {
                duration = UnityEngine.Random.Range(durationMin, durationMax);
            }
            var result = actorAIController.SequenceDuration >= duration;
            if (result)
            {
                duration = 0f;
            }
            return result;
        }
    }
}
