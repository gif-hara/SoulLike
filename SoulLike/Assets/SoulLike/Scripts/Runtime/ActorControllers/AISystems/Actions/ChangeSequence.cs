using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SoulLike.ActorControllers.AISystems.Actions
{
    [Serializable]
    public sealed class ChangeSequence : IAIAction
    {
        [SerializeField]
        private string sequenceName;

        public UniTask InvokeAsync(Actor actor, ActorAIController actorAIController, CancellationToken cancellationToken)
        {
            actorAIController.BeginSequenceAsync(sequenceName).Forget();
            return UniTask.CompletedTask;
        }
    }
}
