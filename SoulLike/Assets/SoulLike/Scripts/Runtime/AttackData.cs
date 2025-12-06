using System;
using UnityEngine;

namespace SoulLike
{
    [Serializable]
    public struct AttackData
    {
        public Collider Collider;

        public float Power;

        public Vector3 Knockback;

        public float KnockbackDuration;

        public AnimationCurve KnockbackCurve;

        public float HitStopDuration;
    }
}
