using System;
using HK;
using UnityEngine;

namespace SoulLike.ActorControllers.AISystems.SelectSequences
{
    public class Weight : ISelectSequence
    {
        [SerializeField]
        private Element[] elements;

        public string Select(Actor actor, ActorAIController actorAIController)
        {
            return elements.Lottery(e => e.Weight).SequenceName;
        }

        [Serializable]
        public class Element
        {
            [field: SerializeField]
            public string SequenceName { get; private set; }

            [field: SerializeField, Min(0f)]
            public int Weight { get; private set; }
        }
    }
}
