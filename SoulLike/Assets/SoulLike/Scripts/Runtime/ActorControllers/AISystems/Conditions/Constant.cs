using System;
using UnityEngine;

namespace SoulLike.ActorControllers.AISystems.Conditions
{
    [Serializable]
    public sealed class Constant : IAICondition
    {
        [SerializeField]
        private bool value;

        public bool Evaluate(Actor actor, ActorAIController actorAIController)
        {
            return value;
        }
    }
}
