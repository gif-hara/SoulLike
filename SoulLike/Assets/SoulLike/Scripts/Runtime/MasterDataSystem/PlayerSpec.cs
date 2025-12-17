using UnityEngine;

namespace SoulLike.MasterDataSystem
{
    [System.Serializable]
    public sealed class PlayerSpec
    {
        [field: SerializeField]
        public ActorStatusSpec ActorStatusSpec { get; private set; }

        [field: SerializeField]
        public float MoveSpeed { get; private set; }

        [field: SerializeField]
        public float MoveAcceleration { get; private set; }

        [field: SerializeField]
        public float RotateSpeed { get; private set; }

        [field: SerializeField]
        public Weapon WeaponPrefab { get; private set; }

        [field: SerializeField]
        public int[] StrongAttackUniqueAttackIds { get; private set; }

        [field: SerializeField]
        public int[] ParryUniqueAttackIds { get; private set; }
    }
}
