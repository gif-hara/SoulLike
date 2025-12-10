using System;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.Assertions;

namespace SoulLike.ActorControllers.Abilities
{
    public sealed class ActorAnimation : IActorAbility
    {
        private Animator animator;

        private AnimatorOverrideController overrideController;

        private readonly AnimatorParameter.DictionaryList animatorParameters = new();

        private int currentAttackId = 1;

        public static class Parameter
        {
            public const string MoveSpeed = "MoveSpeed";

            public const string Attack = "Attack";

            public const string AttackId = "AttackId";

            public const string WeaponId = "WeaponId";

            public const string Dodge = "Dodge";

            public const string Damage = "Damage";

            public const string DamageId = "DamageId";

            public const string LockedOn = "LockedOn";

            public const string MoveX = "MoveX";

            public const string MoveY = "MoveY";

            public const string IsAlive = "IsAlive";

            public const string Dead = "Dead";

            public const string Idle = "Idle";

            public static string GetAttackStateName(int weaponId, int attackId)
            {
                var weaponName = weaponId switch
                {
                    1 => "Hand1",
                    _ => throw new ArgumentOutOfRangeException(nameof(weaponId), weaponId, null)
                };

                return $"Attack_{weaponName}_{attackId}";
            }

            public static string GetDamageStateName(int damageId)
            {
                return $"Damage{damageId}";
            }
        }

        public void Activate(Actor actor)
        {
            var sceneView = actor.GetAbility<ActorSceneViewHandler>().SceneView;
            animator = sceneView.Animator;
            Assert.IsNotNull(animator, $"{nameof(Animator)} is not assigned in {actor.name}.");
            overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
#if UNITY_EDITOR
            if (overrideController != null)
            {
                overrideController = UnityEngine.Object.Instantiate(overrideController);
                animator.runtimeAnimatorController = overrideController;
            }
#endif
            if (overrideController == null)
            {
                overrideController = new AnimatorOverrideController();
                overrideController.runtimeAnimatorController = animator.runtimeAnimatorController;
                animator.runtimeAnimatorController = overrideController;
            }

            sceneView.ActorAnimationEvent.Activate(actor);
            sceneView.ActorAnimatorMove.Activate(actor);

            var actorTime = actor.GetAbility<ActorTime>();
            actorTime.UpdatedTimeScale
                .Subscribe((this, actorTime), static (_, t) =>
                {
                    var (@this, actorTime) = t;
                    @this.animator.speed = actorTime.Time.totalTimeScale;
                })
                .RegisterTo(actor.destroyCancellationToken);
        }

        public void PlayAttackAnimation(AnimationClip animationClip)
        {
            currentAttackId = currentAttackId == 1 ? 2 : 1;
            overrideController[$"Attack.{currentAttackId}"] = animationClip;
            SetInteger(Parameter.AttackId, currentAttackId);
            SetTrigger(Parameter.Attack);
            UpdateAnimator();
        }

        public void PlayDamageAnimation(int damageId)
        {
            SetInteger(Parameter.DamageId, damageId);
            SetTrigger(Parameter.Damage);
            UpdateAnimator();
        }

        public string GetCurrentAttackStateName()
        {
            return $"Attack{currentAttackId}";
        }

        private AnimatorParameter GetParameter(string parameterName)
        {
            if (animatorParameters.TryGetValue(parameterName, out var animatorParameter))
            {
                return animatorParameter;
            }

            animatorParameter = new AnimatorParameter(parameterName);
            animatorParameters.Add(animatorParameter);
            return animatorParameter;
        }

        public void SetBool(string parameterName, bool value)
        {
            var animatorParameter = GetParameter(parameterName);
            animator.SetBool(animatorParameter.Hash, value);
        }

        public void SetTrigger(string parameterName)
        {
            var animatorParameter = GetParameter(parameterName);
            animator.SetTrigger(animatorParameter.Hash);
        }

        public void SetFloat(string parameterName, float value)
        {
            var animatorParameter = GetParameter(parameterName);
            animator.SetFloat(animatorParameter.Hash, value);
        }

        public void SetInteger(string parameterName, int value)
        {
            var animatorParameter = GetParameter(parameterName);
            animator.SetInteger(animatorParameter.Hash, value);
        }

        public void ResetTrigger(string parameterName)
        {
            var animatorParameter = GetParameter(parameterName);
            animator.ResetTrigger(animatorParameter.Hash);
        }

        public void UpdateAnimator()
        {
            animator.Update(0);
        }

        public Observable<ObservableStateMachineTrigger.OnStateInfo> OnStateEnterAsObservable() => animator.GetBehaviour<ObservableStateMachineTrigger>().OnStateEnterAsObservable();

        public Observable<ObservableStateMachineTrigger.OnStateInfo> OnStateEnterAsObservable(string targetStateName) => animator.GetBehaviour<ObservableStateMachineTrigger>().OnStateUpdateAsObservable().Where(targetStateName, static (x, stateName) => x.StateInfo.IsName(stateName));

        public Observable<ObservableStateMachineTrigger.OnStateInfo> OnStateExitAsObservable() => animator.GetBehaviour<ObservableStateMachineTrigger>().OnStateExitAsObservable();

        public Observable<ObservableStateMachineTrigger.OnStateInfo> OnStateExitAsObservable(string targetStateName) => animator.GetBehaviour<ObservableStateMachineTrigger>().OnStateExitAsObservable().Where(targetStateName, static (x, stateName) => x.StateInfo.IsName(stateName));
    }
}
