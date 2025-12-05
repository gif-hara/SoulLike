using System;
using System.Collections.Generic;
using R3;
using SoulLike.ActorControllers;
using SoulLike.ActorControllers.Abilities;
using SoulLike.WeaponActions;
using TNRD;
using UnityEngine;

namespace SoulLike
{
    public sealed class Weapon : MonoBehaviour
    {
#if UNITY_EDITOR
        [ClassesOnly]
#endif
        [SerializeField]
        private List<SerializableInterface<IWeaponAction>> initializeActions = new();

        [SerializeField]
        private List<BasicAttackElement> basicAttackElements = new();

        private Actor actor;

        private ActorAnimation actorAnimation;

        private ActorWeaponHandler actorWeaponHandler;

        private ActorMovement actorMovement;

        public int BasicAttackComboId { get; set; } = 0;

        private IDisposable attackExitProcess;

        public void Initialize(Actor actor)
        {
            this.actor = actor;
            actorAnimation = actor.GetAbility<ActorAnimation>();
            actorWeaponHandler = actor.GetAbility<ActorWeaponHandler>();
            actorMovement = actor.GetAbility<ActorMovement>();
            transform.SetParent(actor.transform, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            foreach (var actionInterface in initializeActions)
            {
                actionInterface.Value.Invoke(this, actor);
            }
        }

        public void InvokeBasicAttack()
        {
            var element = basicAttackElements.Find(x => x.ComboId == BasicAttackComboId);
            if (element == null)
            {
                Debug.LogWarning($"No basic attack element found for combo ID: {BasicAttackComboId}");
                return;
            }

            attackExitProcess?.Dispose();
            attackExitProcess = null;
            actorWeaponHandler.CanAttack.Value = false;
            actorMovement.CanMove.Value = false;
            actorMovement.CanRotate.Value = false;
            actorAnimation.PlayAttackAnimation(element.AnimationClip);
            attackExitProcess = actorAnimation.OnStateExitAsObservable()
                .Subscribe(this, static (x, @this) =>
                {
                    if (x.StateInfo.IsName(@this.actorAnimation.GetCurrentAttackStateName()))
                    {
                        @this.actorMovement.CanMove.Value = true;
                        @this.actorMovement.CanRotate.Value = true;
                        @this.actorWeaponHandler.CanAttack.Value = true;
                        @this.BasicAttackComboId = 0;
                        @this.attackExitProcess?.Dispose();
                        @this.attackExitProcess = null;
                        Debug.Log($"Attack finished");
                    }
                });
            attackExitProcess.RegisterTo(actor.destroyCancellationToken);
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
