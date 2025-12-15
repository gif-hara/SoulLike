using UnityEngine;

namespace SoulLike.ShopActions
{
    public class AddParameter : IShopAction
    {
        public enum ParameterType
        {
            HitPoint,
            Stamina,
            StaminaRecoveryPerSecond,
            AttackRate,
            DamageCutRate,
            AddExperienceRate,
            ParryUniqueAttackId,
            StrongAttackUniqueAttackId
        }

        [SerializeField]
        private ParameterType parameterType;

        [SerializeField]
        private float amount;

        public void Invoke(UserData userData)
        {
            switch (parameterType)
            {
                case ParameterType.HitPoint:
                    userData.AddHitPoint((int)amount);
                    break;
                case ParameterType.Stamina:
                    userData.AddStamina((int)amount);
                    break;
                case ParameterType.StaminaRecoveryPerSecond:
                    userData.AddStaminaRecoveryPerSecond(amount);
                    break;
                case ParameterType.AttackRate:
                    userData.AddAttackRate(amount);
                    break;
                case ParameterType.DamageCutRate:
                    userData.AddDamageCutRate(amount);
                    break;
                case ParameterType.AddExperienceRate:
                    userData.AddAqcuireExperienceRate(amount);
                    break;
                case ParameterType.ParryUniqueAttackId:
                    userData.SetParryUniqueAttackId((int)amount);
                    break;
                case ParameterType.StrongAttackUniqueAttackId:
                    userData.SetStrongAttackUniqueAttackId((int)amount);
                    break;
            }
        }
    }
}
