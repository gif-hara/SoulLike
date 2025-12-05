using System;
using System.Collections.Generic;
using SoulLike.ActorControllers;
using SoulLike.ActorControllers.Abilities;
using SoulLike.WeaponActions;
using TNRD;
using UnityEngine;

namespace SoulLike
{
    public sealed class Weapon : MonoBehaviour
    {
        [SerializeField]
        private List<BasicAttackElement> basicAttackElements = new();

        private Actor actor;

        private ActorWeaponHandler weaponHandler;

        public int BasicAttackComboId { get; set; } = 0;

        public void Initialize(Actor actor, ActorWeaponHandler weaponHandler)
        {
            this.actor = actor;
            this.weaponHandler = weaponHandler;
            transform.SetParent(actor.transform, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public void InvokeBasicAttack()
        {
            var element = basicAttackElements.Find(x => x.ComboId == BasicAttackComboId);
            if (element == null)
            {
                Debug.LogWarning($"No basic attack element found for combo ID: {BasicAttackComboId}");
                return;
            }

            foreach (var actionInterface in element.Actions)
            {
                actionInterface.Value.Invoke(this, actor);
            }
        }

        [Serializable]
        public class BasicAttackElement
        {
            [field: SerializeField]
            public int ComboId { get; private set; }

            [field: SerializeField]
            public AnimationClip AnimationClip { get; private set; }

#if UNITY_EDITOR
            [ClassesOnly]
#endif
            [field: SerializeField]
            public List<SerializableInterface<IWeaponAction>> Actions { get; private set; }
        }
    }
}
