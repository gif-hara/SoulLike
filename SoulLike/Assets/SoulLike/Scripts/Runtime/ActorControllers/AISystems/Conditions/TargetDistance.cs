using System;
using SoulLike.ActorControllers.Abilities;
using UnityEngine;

namespace SoulLike.ActorControllers.AISystems.Conditions
{
    [Serializable]
    public sealed class TargetDistance : IAICondition
    {
        [SerializeField, Min(0f)]
        private float distance;

        public bool Evaluate(Actor actor, ActorAIController actorAIController)
        {
            return actor.GetAbility<ActorTargetHandler>().GetDistanceToTarget() <= distance;
        }
    }
}
