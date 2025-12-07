using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SoulLike.ActorControllers.Abilities;
using SoulLike.ActorControllers.AISystems.Conditions;
using TNRD;
using UnityEngine;

namespace SoulLike.ActorControllers.AISystems.Actions
{
    [Serializable]
    public sealed class Walk : IAIAction
    {
        [SerializeField, Min(0f)]
        private Vector2 velocity;

        [SerializeField]
        private SerializableInterface<IAICondition> completeCondition;

        public async UniTask InvokeAsync(Actor actor, ActorAIController actorAIController, CancellationToken cancellationToken)
        {
            var actorWalk = actor.GetAbility<ActorWalk>();
            while (!cancellationToken.IsCancellationRequested && !completeCondition.Value.Evaluate(actor, actorAIController))
            {
                var fixedVelocity = actor.transform.TransformDirection(new Vector3(velocity.x, 0f, velocity.y));
                actorWalk.SetNormalizedVelocity(fixedVelocity);
                await UniTask.Yield(cancellationToken);
            }
        }
    }
}
