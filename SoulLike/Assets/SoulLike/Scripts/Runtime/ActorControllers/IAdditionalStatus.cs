namespace SoulLike.ActorControllers
{
    public interface IAdditionalStatus
    {
        int HitPoint { get; }

        int Stamina { get; }

        float StaminaRecoveryPerSecond { get; }

        float AttackRate { get; }

        float DamageCutRate { get; }

        float AcquireExperienceRate { get; }

        /// <summary>
        /// 回避モーションの攻撃Id
        /// </summary>
        int DodgeUniqueAttackId { get; }

        int StrongAttackUniqueAttackId { get; }
    }
}
