using System;
using System.Collections.Generic;
using System.Threading;
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

        private CancellationTokenSource endAttackCancellationTokenSource;

        private const string AttackStateName = "Attack";

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
                actionInterface.Value.Invoke(this, actor, actor.destroyCancellationToken);
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

            endAttackCancellationTokenSource?.Cancel();
            endAttackCancellationTokenSource?.Dispose();
            endAttackCancellationTokenSource = new CancellationTokenSource();
            actorWeaponHandler.AttackBlocker.Block(AttackStateName);
            actorMovement.MoveBlocker.Block(AttackStateName);
            actorMovement.RotateBlocker.Block(AttackStateName);
            BasicAttackComboId = element.NextComboId;
            actorAnimation.PlayAttackAnimation(element.AnimationClip);
            actorAnimation.OnStateExitAsObservable()
                .Subscribe((this, element), static (x, t) =>
                {
                    var (@this, element) = t;
                    if (x.StateInfo.IsName(@this.actorAnimation.GetCurrentAttackStateName()))
                    {
                        @this.actorMovement.MoveBlocker.Unblock(AttackStateName);
                        @this.actorMovement.RotateBlocker.Unblock(AttackStateName);
                        @this.actorWeaponHandler.AttackBlocker.Unblock(AttackStateName);
                        @this.BasicAttackComboId = 0;
                        @this.endAttackCancellationTokenSource?.Cancel();
                        @this.endAttackCancellationTokenSource?.Dispose();
                        @this.endAttackCancellationTokenSource = null;
                        foreach (var attackElement in element.AttackElements)
                        {
                            foreach (var activeObject in attackElement.ActiveObjects)
                            {
                                activeObject.SetActive(false);
                            }
                        }
                    }
                })
                .RegisterTo(endAttackCancellationTokenSource.Token);
            foreach (var attackElement in element.AttackElements)
            {
                actor.Event.Broker.Receive<ActorEvent.BeginAttack>()
                    .Where(attackElement.AttackId, static (x, attackId) => x.AttackId == attackId)
                    .Subscribe((attackElement, this, actor, element), static (_, t) =>
                    {
                        var (attackElement, @this, actor, element) = t;
                        foreach (var activeObject in attackElement.ActiveObjects)
                        {
                            activeObject.SetActive(true);
                        }
                        foreach (var actionInterface in attackElement.BeginAttackActions)
                        {
                            actionInterface.Value.Invoke(@this, actor, @this.endAttackCancellationTokenSource.Token);
                        }
                    })
                    .RegisterTo(endAttackCancellationTokenSource.Token);
                actor.Event.Broker.Receive<ActorEvent.EndAttack>()
                    .Where(attackElement.AttackId, static (x, attackId) => x.AttackId == attackId)
                    .Subscribe((attackElement, this, actor, element), static (_, t) =>
                    {
                        var (attackElement, @this, actor, element) = t;
                        foreach (var activeObject in attackElement.ActiveObjects)
                        {
                            activeObject.SetActive(false);
                        }
                        foreach (var actionInterface in attackElement.EndAttackActions)
                        {
                            actionInterface.Value.Invoke(@this, actor, @this.endAttackCancellationTokenSource.Token);
                        }
                    })
                    .RegisterTo(endAttackCancellationTokenSource.Token);
            }
            foreach (var action in element.Actions)
            {
                action.Value.Invoke(this, actor, endAttackCancellationTokenSource.Token);
            }
        }

        [Serializable]
        public class BasicAttackElement
        {
            [field: SerializeField]
            public int ComboId { get; private set; }

            [field: SerializeField]
            public int NextComboId { get; private set; }

            [field: SerializeField]
            public AnimationClip AnimationClip { get; private set; }

            [field: SerializeField]
            public List<AttackElement> AttackElements { get; private set; }

#if UNITY_EDITOR
            [ClassesOnly]
#endif
            [field: SerializeField]
            public List<SerializableInterface<IWeaponAction>> Actions { get; private set; }
        }

        [Serializable]
        public class AttackElement
        {
            [field: SerializeField]
            public int AttackId { get; private set; }

            [field: SerializeField]
            public List<GameObject> ActiveObjects { get; private set; }

#if UNITY_EDITOR
            [ClassesOnly]
#endif
            [field: SerializeField]
            public List<SerializableInterface<IWeaponAction>> BeginAttackActions { get; private set; }

#if UNITY_EDITOR
            [ClassesOnly]
#endif
            [field: SerializeField]
            public List<SerializableInterface<IWeaponAction>> EndAttackActions { get; private set; }
        }
    }
}
