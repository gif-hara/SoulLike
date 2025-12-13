using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using SoulLike.ActorControllers.Abilities;
using UnityEngine;

namespace SoulLike.ActorControllers.AISystems.Actions
{
    [Serializable]
    public sealed class WaitOnStateEnter : IAIAction
    {
        [SerializeField]
        private string stateName;

        public async UniTask InvokeAsync(Actor actor, ActorAIController actorAIController, CancellationToken cancellationToken)
        {
            Debug.Log($"WaitOnStateEnter Action Invoked for state: {stateName}");
            await actor.GetAbility<ActorAnimation>().OnStateEnterAsObservable(stateName).FirstAsync(cancellationToken).AsUniTask();
            Debug.Log($"WaitOnStateEnter Action Completed for state: {stateName}");
        }
    }
}
