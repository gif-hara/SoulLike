using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SoulLike.ActorControllers.AISystems.Actions
{
    [Serializable]
    public sealed class ChangeAI : IAIAction
    {
        [SerializeField]
        private ActorAI newAI;

        public UniTask InvokeAsync(Actor actor, ActorAIController actorAIController, CancellationToken cancellationToken)
        {
            actorAIController.Change(newAI);
            return UniTask.CompletedTask;
        }
    }
}
