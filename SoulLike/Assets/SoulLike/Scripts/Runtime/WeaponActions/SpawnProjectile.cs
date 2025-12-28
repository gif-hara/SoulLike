using System;
using System.Threading;
using SoulLike.ActorControllers;
using UnityEngine;

namespace SoulLike.WeaponActions
{
    [Serializable]
    public sealed class SpawnProjectile : IWeaponAction
    {
        [SerializeField]
        private Projectile projectilePrefab;

        [SerializeField]
        private AttackData attackData;

        public void Invoke(Weapon weapon, Actor actor, CancellationToken scope)
        {
            var projectile = UnityEngine.Object.Instantiate(projectilePrefab, actor.transform.position, actor.transform.rotation);
            projectile.Activate(actor, attackData, weapon.gameObject.layer);
        }
    }
}
