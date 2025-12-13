namespace SoulLike.ActorControllers
{
    public interface IAdditionalStatus
    {
        int HitPoint { get; }

        int Stamina { get; }

        float StaminaRecoveryPerSecond { get; }

        float AttackRate { get; }

        float DefenseRate { get; }
    }
}
