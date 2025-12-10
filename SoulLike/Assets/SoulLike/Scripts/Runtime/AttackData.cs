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

        public Vector3 SceneViewShakeStartValue;

        public Vector3 SceneViewShakeStrength;

        public float SceneViewShakeDuration;

        public int SceneViewShakeFrequency;

        public int SceneViewShakeDampingRatio;

        public string SfxKey;
    }
}
