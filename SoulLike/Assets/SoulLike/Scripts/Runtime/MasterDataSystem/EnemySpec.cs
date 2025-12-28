using System.Collections.Generic;
using SoulLike.ActorControllers.ActorActions;
using SoulLike.ActorControllers.AISystems;
using TNRD;
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
        public float MoveAcceleration { get; private set; }

        [field: SerializeField]
        public float RotateSpeed { get; private set; }

        [field: SerializeField]
        public Weapon WeaponPrefab { get; private set; }

        [field: SerializeField]
        public ActorAI ActorAI { get; private set; }

        [field: SerializeField]
        public List<ActorAction> OnDeadActions { get; private set; } = new();

        [field: SerializeField]
        public List<ActorAction> OnRestartActions { get; private set; } = new();
    }
}
