using System;
using UnityEngine;

namespace SoulLike.ActorControllers
{
    [Serializable]
    public sealed class SerializableAdditionalStatus : IAdditionalStatus
    {
        [SerializeField]
        private int hitPoint;

        [SerializeField]
        private int stamina;

        [SerializeField]
        private float staminaRecoveryPerSecond;

        [SerializeField]
        private float attackRate;

        [SerializeField]
        private float damageCutRate;

        [SerializeField]
        private float acquireExperienceRate;

        [SerializeField]
        private int dodgeUniqueAttackId;

        [SerializeField]
        private int strongAttackUniqueAttackId;

        [SerializeField]
        private int parryUniqueAttackId;

        public int HitPoint => hitPoint;

        public int Stamina => stamina;

        public float StaminaRecoveryPerSecond => staminaRecoveryPerSecond;

        public float AttackRate => attackRate;

        public float DamageCutRate => damageCutRate;

        public float AcquireExperienceRate => acquireExperienceRate;

        public int DodgeUniqueAttackId => dodgeUniqueAttackId;

        public int StrongAttackUniqueAttackId => strongAttackUniqueAttackId;

        public int ParryUniqueAttackId => parryUniqueAttackId;
    }
}
