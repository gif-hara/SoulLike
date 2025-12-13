using R3;
using SoulLike.ActorControllers;

namespace SoulLike
{
    public class UserData : IAdditionalStatus
    {
        private readonly ReactiveProperty<int> experience = new(0);

        public ReadOnlyReactiveProperty<int> Experience => experience;

        private readonly ReactiveProperty<int> hitPoint = new();

        public ReadOnlyReactiveProperty<int> HitPoint => hitPoint;

        private readonly ReactiveProperty<int> stamina = new();

        public ReadOnlyReactiveProperty<int> Stamina => stamina;

        private readonly ReactiveProperty<float> staminaRecoveryPerSecond = new();

        public ReadOnlyReactiveProperty<float> StaminaRecoveryPerSecond => staminaRecoveryPerSecond;

        private readonly ReactiveProperty<float> attackRate = new();

        public ReadOnlyReactiveProperty<float> AttackRate => attackRate;

        private readonly ReactiveProperty<float> damageCutRate = new();

        public ReadOnlyReactiveProperty<float> DamageCutRate => damageCutRate;

        int IAdditionalStatus.HitPoint => hitPoint.Value;

        int IAdditionalStatus.Stamina => stamina.Value;

        float IAdditionalStatus.StaminaRecoveryPerSecond => staminaRecoveryPerSecond.Value;

        float IAdditionalStatus.AttackRate => attackRate.Value;

        float IAdditionalStatus.DamageCutRate => damageCutRate.Value;

        public void AddExperience(int amount)
        {
            experience.Value += amount;
        }

        public void AddHitPoint(int amount)
        {
            hitPoint.Value += amount;
        }

        public void AddStamina(int amount)
        {
            stamina.Value += amount;
        }

        public void AddStaminaRecoveryPerSecond(float amount)
        {
            staminaRecoveryPerSecond.Value += amount;
        }

        public void AddAttackRate(float amount)
        {
            attackRate.Value += amount;
        }

        public void AddDamageCutRate(float amount)
        {
            damageCutRate.Value += amount;
        }
    }
}
