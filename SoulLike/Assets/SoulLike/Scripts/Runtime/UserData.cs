using System.Collections.Generic;
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

        private readonly ReactiveProperty<float> attackRate = new(1.0f);

        public ReadOnlyReactiveProperty<float> AttackRate => attackRate;

        private readonly ReactiveProperty<float> damageCutRate = new();

        public ReadOnlyReactiveProperty<float> DamageCutRate => damageCutRate;

        private readonly ReactiveProperty<float> aqcuireExperienceRate = new(1.0f);

        public ReadOnlyReactiveProperty<float> AqcuireExperienceRate => aqcuireExperienceRate;

        private readonly Dictionary<string, int> purchasedShopElementCounts = new();

        private int dodgeUniqueAttackId = 0;

        private int strongAttackUniqueAttackId = 0;

        private int parryUniqueAttackId = 0;

        int IAdditionalStatus.HitPoint => hitPoint.Value;

        int IAdditionalStatus.Stamina => stamina.Value;

        float IAdditionalStatus.StaminaRecoveryPerSecond => staminaRecoveryPerSecond.Value;

        float IAdditionalStatus.AttackRate => attackRate.Value;

        float IAdditionalStatus.DamageCutRate => damageCutRate.Value;

        float IAdditionalStatus.AcquireExperienceRate => aqcuireExperienceRate.Value;

        public int DodgeUniqueAttackId => dodgeUniqueAttackId;

        public int StrongAttackUniqueAttackId => strongAttackUniqueAttackId;

        public int ParryUniqueAttackId => parryUniqueAttackId;

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

        public void AddAqcuireExperienceRate(float amount)
        {
            aqcuireExperienceRate.Value += amount;
        }

        public void SetDodgeUniqueAttackId(int attackId)
        {
            dodgeUniqueAttackId = attackId;
        }

        public void SetStrongAttackUniqueAttackId(int attackId)
        {
            strongAttackUniqueAttackId = attackId;
        }

        public void SetParryUniqueAttackId(int attackId)
        {
            parryUniqueAttackId = attackId;
        }

        public void AddPurchasedShopElementCount(string shopElementId, int priceIndex)
        {
            purchasedShopElementCounts[shopElementId] = priceIndex;
        }

        public int GetPurchasedShopElementCount(string shopElementId)
        {
            if (!purchasedShopElementCounts.ContainsKey(shopElementId))
            {
                return 0;
            }
            return purchasedShopElementCounts[shopElementId];
        }
    }
}
