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

        private readonly AnimatorParameter.DictionaryList animatorParameters = new();

        public static class Parameter
        {
            public const string MoveSpeed = "MoveSpeed";

            public const string Attack = "Attack";

            public const string AttackId = "AttackId";

            public const string WeaponId = "WeaponId";

            public static string GetAttackStateName(int weaponId, int attackId)
            {
                var weaponName = weaponId switch
                {
                    1 => "Hand1",
                    _ => throw new ArgumentOutOfRangeException(nameof(weaponId), weaponId, null)
                };

                return $"Attack_{weaponName}_{attackId}";
            }
        }

        public void Activate(Actor actor)
        {
            var sceneView = actor.GetComponentInChildren<SceneView>();
            Assert.IsNotNull(sceneView, $"{nameof(SceneView)} is not assigned in {actor.name}.");
            animator = sceneView.Animator;
            Assert.IsNotNull(animator, $"{nameof(Animator)} is not assigned in {actor.name}.");

            sceneView.ActorAnimationEvent.Activate(actor);
            sceneView.ActorAnimatorMove.Activate(actor);
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

        public Observable<ObservableStateMachineTrigger.OnStateInfo> OnStateExitAsObservable() => animator.GetBehaviour<ObservableStateMachineTrigger>().OnStateExitAsObservable();
    }
}
