using System;
using UnityEngine;

namespace SoulLike.MasterDataSystem
{
    [Serializable]
    public sealed class ActorStatusSpec
    {
        [field: SerializeField]
        public float HitPoint { get; private set; }
    }
}
