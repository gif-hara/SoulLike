using System.Collections.Generic;
using TNRD;
using UnityEngine;

namespace SoulLike.ActorControllers.AISystems
{
    [CreateAssetMenu(fileName = "AISequence", menuName = "SoulLike/AI/AISequence")]
    public class AISequence : ScriptableObject
    {
        [field: SerializeField]
        public string SequenceName { get; private set; }

#if UNITY_EDITOR
        [ClassesOnly]
#endif
        [field: SerializeField]
        public List<SerializableInterface<IAIAction>> Actions { get; private set; } = new();
    }
}
