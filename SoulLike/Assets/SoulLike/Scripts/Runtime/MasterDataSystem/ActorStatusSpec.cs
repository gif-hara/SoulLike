using System;
using SoulLike.ActorControllers.ActorActions;
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

        [field: SerializeField]
        public float DodgeStaminaCost { get; private set; }

        [field: SerializeField]
        public float StunResistance { get; private set; }

        [field: SerializeField]
        public float StunDuration { get; private set; }

        [field: SerializeField]
        public ActorAction OnStunAction { get; private set; }

        [field: SerializeField]
        public int SpecialStockMax { get; private set; }

        [field: SerializeField]
        public ActorAction OnSpecialStockReached { get; private set; }

        [field: SerializeField]
        public float AttackBuffRate { get; private set; }
    }
}
