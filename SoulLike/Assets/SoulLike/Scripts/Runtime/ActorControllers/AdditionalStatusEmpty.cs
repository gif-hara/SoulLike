namespace SoulLike.ActorControllers
{
    public sealed class AdditionalStatusEmpty : IAdditionalStatus
    {
        public int HitPoint => 0;

        public int Stamina => 0;

        public float StaminaRecoveryPerSecond => 0.0f;

        public float AttackRate => 1f;

        public float DamageCutRate => 1f;
    }
}
