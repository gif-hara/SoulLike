using SoulLike.ActorControllers.AISystems;
using UnityEngine;

namespace SoulLike.MasterDataSystem
{
    [System.Serializable]
    public sealed class EnemySpec
    {
        [field: SerializeField]
        public ActorStatusSpec ActorStatusSpec { get; private set; }

        [field: SerializeField]
        public float MoveSpeed { get; private set; }

        [field: SerializeField]
        public float RotateSpeed { get; private set; }

        [field: SerializeField]
        public Weapon WeaponPrefab { get; private set; }

        [field: SerializeField]
        public ActorAI ActorAI { get; private set; }
    }
}
