using R3;

namespace SoulLike.ActorControllers.Abilities
{
    public sealed class ActorWeaponHandler : IActorAbility
    {
        private Actor actor;

        public readonly ReactiveProperty<bool> CanAttack = new(true);

        private Weapon currentWeapon;

        public void Activate(Actor actor)
        {
            this.actor = actor;
        }

        public void CreateWeapon(Weapon weaponPrefab)
        {
            if (currentWeapon != null)
            {
                UnityEngine.Object.Destroy(currentWeapon.gameObject);
            }
            var weaponObject = UnityEngine.Object.Instantiate(weaponPrefab);
            weaponObject.Initialize(actor);
            currentWeapon = weaponObject;
        }

        public bool TryBasicAttack()
        {
            if (!CanAttack.Value)
            {
                return false;
            }

            currentWeapon.InvokeBasicAttack();
            return true;
        }
    }
}
