using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulLike
{
    [Serializable]
    public struct AttackData
    {
        public float Power;

        public float StunDamage;

        public int EarnedExperience;

        public float SpecialPowerRecovery;

        public int DamageId;

        public float HitStopDuration;

        public float HitStopTimeScale;

        public Vector3 SceneViewShakeStartValue;

        public Vector3 SceneViewShakeStrength;

        public float SceneViewShakeDuration;

        public int SceneViewShakeFrequency;

        public int SceneViewShakeDampingRatio;

        public string SfxKey;

        public string SfxKeyOnStun;

        public List<GameObject> HitEffectPrefabs;
    }
}
