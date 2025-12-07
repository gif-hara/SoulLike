using System;
using UnityEngine;

namespace SoulLike.MasterDataSystem
{
    [Serializable]
    public sealed class ActorStatusSpec
    {
        [field: SerializeField]
        public float HitPoint { get; private set; }

        [field: SerializeField]
        public float Stamina { get; private set; }

        [field: SerializeField]
        public float StaminaRecoveryPerSecond { get; private set; }
    }
}
