using HK;

namespace SoulLike.ActorControllers.Abilities
{
    public sealed class ActorWeaponHandler : IActorAbility
    {
        private Actor actor;

        public readonly Blocker AttackBlocker = new();

        private Weapon currentWeapon;

        public void Activate(Actor actor)
        {
            this.actor = actor;
        }

        public void CreateWeapon(Weapon weaponPrefab, int layer)
        {
            if (currentWeapon != null)
            {
                UnityEngine.Object.Destroy(currentWeapon.gameObject);
            }
            var weaponObject = UnityEngine.Object.Instantiate(weaponPrefab);
            weaponObject.gameObject.SetLayerRecursive(layer);
            weaponObject.Initialize(actor);
            currentWeapon = weaponObject;
        }

        public bool TryBasicAttack()
        {
            if (AttackBlocker.IsBlocked)
            {
                return false;
            }

            return currentWeapon.TryInvokeBasicAttack();
        }

        public bool TryUniqueAttack(int uniqueAttackId)
        {
            if (AttackBlocker.IsBlocked)
            {
                return false;
            }

            return currentWeapon.TryInvokeUniqueAttack(uniqueAttackId);
        }

        public void ForceInvokeUniqueAttack(int uniqueAttackId)
        {
            currentWeapon.ForceInvokeUniqueAttack(uniqueAttackId);
        }
    }
}
