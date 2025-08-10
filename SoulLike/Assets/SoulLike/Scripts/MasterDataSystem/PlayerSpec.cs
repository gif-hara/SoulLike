using UnityEngine;

namespace SoulLike.MasterDataSystem
{
    [System.Serializable]
    public sealed class PlayerSpec
    {
        [field: SerializeField]
        public float MoveSpeed { get; private set; }

        [field: SerializeField]
        public float RotateSpeed { get; private set; }
    }
}
