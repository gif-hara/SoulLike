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
        }

        public void Activate(Actor actor)
        {
            var sceneView = actor.GetComponentInChildren<SceneView>();
            Assert.IsNotNull(sceneView, $"{nameof(SceneView)} is not assigned in {actor.name}.");
            animator = sceneView.Animator;
            Assert.IsNotNull(animator, $"{nameof(Animator)} is not assigned in {actor.name}.");
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

        public void SetInt(string parameterName, int value)
        {
            var animatorParameter = GetParameter(parameterName);
            animator.SetInteger(animatorParameter.Hash, value);
        }

        public void ResetTrigger(string parameterName)
        {
            var animatorParameter = GetParameter(parameterName);
            animator.ResetTrigger(animatorParameter.Hash);
        }
    }
}
