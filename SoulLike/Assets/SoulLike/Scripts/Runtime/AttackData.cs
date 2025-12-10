using System;
using UnityEngine;

namespace SoulLike
{
    [Serializable]
    public struct AttackData
    {
        public Collider Collider;

        public float Power;

        public int DamageId;

        public float HitStopDuration;

        public float HitStopTimeScale;
    }
}
