using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulLike.ActorControllers.AISystems
{
    [CreateAssetMenu(fileName = "ActorAI", menuName = "SoulLike/AI/ActorAI")]
    public sealed class ActorAI : ScriptableObject
    {
        [field: SerializeField]
        public string InitialSequenceName { get; private set; }

        [field: SerializeField]
        public string OnStunSequenceName { get; private set; }

        [field: SerializeField]
        public List<AISequence> Sequences { get; private set; } = new();

        public AISequence GetSequence(string sequenceName)
        {
            foreach (var sequence in Sequences)
            {
                if (sequence.SequenceName == sequenceName)
                {
                    return sequence;
                }
            }

            throw new Exception($"AISequence not found: {sequenceName}");
        }
    }
}
