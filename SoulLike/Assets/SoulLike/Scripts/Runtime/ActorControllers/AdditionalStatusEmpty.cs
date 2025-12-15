namespace SoulLike.ActorControllers
{
    public sealed class AdditionalStatusEmpty : IAdditionalStatus
    {
        public int HitPoint => 0;

        public int Stamina => 0;

        public float StaminaRecoveryPerSecond => 0.0f;

        public float AttackRate => 1f;

        public float DamageCutRate => 0f;

        public float AcquireExperienceRate => 1.0f;

        public int DodgeUniqueAttackId => 0;

        public int StrongAttackUniqueAttackId => 0;

        public int ParryUniqueAttackId => 0;
    }
}
