using System;
using System.Collections.Generic;
using System.Threading;
using HK;
using R3;
using R3.Triggers;
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

        [SerializeField]
        private List<UniqueAttackElement> uniqueAttackElements = new();

        private Actor actor;

        private ActorAnimation actorAnimation;

        private ActorWeaponHandler actorWeaponHandler;

        private ActorMovement actorMovement;

        private ActorStatus actorStatus;

        private ActorDodge actorDodge;

        public int BasicAttackComboId { get; set; } = 0;

        private Dictionary<int, HashSet<Actor>> hitActors = new();

        private CancellationTokenSource endAttackAnimationCancellationTokenSource;

        private const string AttackStateName = "Attack";

        public void Initialize(Actor actor)
        {
            this.actor = actor;
            actorAnimation = actor.GetAbility<ActorAnimation>();
            actorWeaponHandler = actor.GetAbility<ActorWeaponHandler>();
            actorMovement = actor.GetAbility<ActorMovement>();
            actorStatus = actor.GetAbility<ActorStatus>();
            actorDodge = actor.GetAbility<ActorDodge>();
            transform.SetParent(actor.transform, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            foreach (var actionInterface in initializeActions)
            {
                actionInterface.Value.Invoke(this, actor, actor.destroyCancellationToken);
            }
        }

        public bool TryInvokeBasicAttack()
        {
            if (!actorStatus.CanUseStamina())
            {
                return false;
            }
            var element = basicAttackElements.Find(x => x.ComboId == BasicAttackComboId);
            if (element == null)
            {
                Debug.LogWarning($"No basic attack element found for combo ID: {BasicAttackComboId}");
                return false;
            }
            BasicAttackComboId = element.NextComboId;
            InvokeAttack(element.StaminaCost, element.AnimationClip, element.AttackElements, element.Actions);
            return true;
        }

        public bool TryInvokeUniqueAttack(int index)
        {
            if (!actorStatus.CanUseStamina())
            {
                return false;
            }
            if (index < 0 || index >= uniqueAttackElements.Count)
            {
                Debug.LogWarning($"Unique attack index {index} is out of range.");
                return false;
            }
            var element = uniqueAttackElements[index];
            InvokeAttack(element.StaminaCost, element.AnimationClip, element.AttackElements, element.Actions);
            return true;
        }

        public void ForceInvokeUniqueAttack(int uniqueAttackId)
        {
            if (uniqueAttackId < 0 || uniqueAttackId >= uniqueAttackElements.Count)
            {
                Debug.LogWarning($"Unique attack index {uniqueAttackId} is out of range.");
                return;
            }
            var element = uniqueAttackElements[uniqueAttackId];
            InvokeAttack(element.StaminaCost, element.AnimationClip, element.AttackElements, element.Actions);
        }

        private void InvokeAttack(float staminaCost, AnimationClip animationClip, List<AttackElement> attackElements, List<SerializableInterface<IWeaponAction>> actions)
        {
            actorStatus.UseStamina(staminaCost);
            endAttackAnimationCancellationTokenSource?.Cancel();
            endAttackAnimationCancellationTokenSource?.Dispose();
            endAttackAnimationCancellationTokenSource = new CancellationTokenSource();
            actorWeaponHandler.AttackBlocker.Block(AttackStateName);
            actorMovement.MoveBlocker.Block(AttackStateName);
            actorMovement.RotateBlocker.Block(AttackStateName);
            actorStatus.StaminaRecoveryBlocker.Block(AttackStateName);
            actorDodge.DodgeBlocker.Block(AttackStateName);
            actorAnimation.PlayAttackAnimation(animationClip);
            actorAnimation.OnStateExitAsObservable(actorAnimation.GetCurrentAttackStateName())
                .Subscribe((this, attackElements), static (x, t) =>
                {
                    var (@this, attackElements) = t;
                    @this.actorMovement.MoveBlocker.Unblock(AttackStateName);
                    @this.actorMovement.RotateBlocker.Unblock(AttackStateName);
                    @this.actorWeaponHandler.AttackBlocker.Unblock(AttackStateName);
                    @this.actorStatus.StaminaRecoveryBlocker.Unblock(AttackStateName);
                    @this.actorDodge.DodgeBlocker.Unblock(AttackStateName);
                    @this.BasicAttackComboId = 0;
                    @this.endAttackAnimationCancellationTokenSource?.Cancel();
                    @this.endAttackAnimationCancellationTokenSource?.Dispose();
                    @this.endAttackAnimationCancellationTokenSource = null;
                    foreach (var attackElement in attackElements)
                    {
                        foreach (var activeObject in attackElement.ActiveObjects)
                        {
                            activeObject.SetActive(false);
                        }
                        foreach (var trail in attackElement.Trails)
                        {
                            trail.Emit = false;
                        }
                    }
                })
                .RegisterTo(endAttackAnimationCancellationTokenSource.Token);
            foreach (var attackElement in attackElements)
            {
                actor.Event.Broker.Receive<ActorEvent.BeginAttack>()
                    .Where(attackElement.AttackId, static (x, attackId) => x.AttackId == attackId)
                    .Subscribe((attackElement, this, actor), static (_, t) =>
                    {
                        var (attackElement, @this, actor) = t;
                        foreach (var activeObject in attackElement.ActiveObjects)
                        {
                            activeObject.SetActive(true);
                        }
                        foreach (var trail in attackElement.Trails)
                        {
                            trail.Emit = true;
                        }
                        foreach (var actionInterface in attackElement.BeginAttackActions)
                        {
                            actionInterface.Value.Invoke(@this, actor, @this.endAttackAnimationCancellationTokenSource.Token);
                        }
                        if (!@this.hitActors.ContainsKey(attackElement.AttackId))
                        {
                            @this.hitActors[attackElement.AttackId] = new HashSet<Actor>();
                        }
                        else
                        {
                            @this.hitActors[attackElement.AttackId].Clear();
                        }
                        attackElement.AttackData.Collider.OnTriggerStayAsObservable()
                            .Subscribe((@this, attackElement, actor, @this.hitActors[attackElement.AttackId]), static (x, t) =>
                            {
                                var (@this, attackElement, actor, hitActors) = t;
                                var target = x.attachedRigidbody?.GetComponent<Actor>();
                                if (target == null)
                                {
                                    return;
                                }
                                if (hitActors.Contains(target))
                                {
                                    return;
                                }
                                var targetStatus = target.GetAbility<ActorStatus>();
                                if (targetStatus == null)
                                {
                                    return;
                                }
                                if (targetStatus.IsInvincible.CurrentValue)
                                {
                                    return;
                                }
                                hitActors.Add(target);
                                if (targetStatus.IsParrying)
                                {
                                    target.Event.Broker.Publish(new ActorEvent.OnBeginParry());
                                }
                                else
                                {
                                    targetStatus.TakeDamage(actor, attackElement.AttackData);
                                    actor.Event.Broker.Publish(new ActorEvent.OnGiveDamage(attackElement.AttackData, targetStatus.IsStunned));
                                    if (!string.IsNullOrEmpty(attackElement.AttackData.SfxKey))
                                    {
                                        TinyServiceLocator.Resolve<AudioManager>().PlaySfx(attackElement.AttackData.SfxKey);
                                    }
                                    if (targetStatus.IsStunned)
                                    {
                                        if (!string.IsNullOrEmpty(attackElement.AttackData.SfxKeyOnStun))
                                        {
                                            TinyServiceLocator.Resolve<AudioManager>().PlaySfx(attackElement.AttackData.SfxKeyOnStun);
                                        }
                                    }
                                    foreach (var hitEffectPrefab in attackElement.HitEffectPrefabs)
                                    {
                                        Instantiate(hitEffectPrefab, x.ClosestPoint(attackElement.AttackData.Collider.transform.position), Quaternion.identity);
                                    }
                                }
                            })
                            .RegisterTo(@this.endAttackAnimationCancellationTokenSource.Token);
                    })
                    .RegisterTo(endAttackAnimationCancellationTokenSource.Token);
                actor.Event.Broker.Receive<ActorEvent.EndAttack>()
                    .Where(attackElement.AttackId, static (x, attackId) => x.AttackId == attackId)
                    .Subscribe((attackElement, this, actor), static (_, t) =>
                    {
                        var (attackElement, @this, actor) = t;
                        foreach (var activeObject in attackElement.ActiveObjects)
                        {
                            activeObject.SetActive(false);
                        }
                        foreach (var trail in attackElement.Trails)
                        {
                            trail.Emit = false;
                        }
                        foreach (var actionInterface in attackElement.EndAttackActions)
                        {
                            actionInterface.Value.Invoke(@this, actor, @this.endAttackAnimationCancellationTokenSource.Token);
                        }
                    })
                    .RegisterTo(endAttackAnimationCancellationTokenSource.Token);
            }
            foreach (var action in actions)
            {
                action.Value.Invoke(this, actor, endAttackAnimationCancellationTokenSource.Token);
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
            public float StaminaCost { get; private set; }

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
        public class UniqueAttackElement
        {
            [field: SerializeField]
            public float StaminaCost { get; private set; }

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

            [field: SerializeField]
            public List<MeleeWeaponTrail> Trails { get; private set; }

            [field: SerializeField]
            public List<GameObject> HitEffectPrefabs { get; private set; }

            [field: SerializeField]
            public AttackData AttackData { get; private set; }

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
