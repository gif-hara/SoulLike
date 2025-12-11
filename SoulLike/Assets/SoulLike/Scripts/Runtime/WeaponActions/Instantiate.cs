using System;
using System.Threading;
using SoulLike.ActorControllers;
using UnityEngine;

namespace SoulLike.WeaponActions
{
    [Serializable]
    public sealed class Instantiate : IWeaponAction
    {
        [SerializeField]
        private GameObject prefab;

        [SerializeField]
        private Vector3 offsetPosition;

        [SerializeField]
        private Vector3 offsetRotation;

        [SerializeField]
        private Vector3 scale = Vector3.one;

        public void Invoke(Weapon weapon, Actor actor, CancellationToken scope)
        {
            if (prefab == null)
            {
                Debug.LogWarning("Instantiate action invoked with null prefab.");
                return;
            }

            var spawnPosition = actor.transform.position + actor.transform.TransformVector(offsetPosition);
            var spawnRotation = actor.transform.rotation * Quaternion.Euler(offsetRotation);

            var instance = UnityEngine.Object.Instantiate(prefab, spawnPosition, spawnRotation);
            instance.transform.localScale = scale;
        }
    }
}
