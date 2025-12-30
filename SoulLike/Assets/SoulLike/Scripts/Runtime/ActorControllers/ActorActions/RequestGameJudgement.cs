using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SoulLike.ActorControllers.ActorActions
{
    [Serializable]
    public sealed class RequestGameJudgement : IActorAction
    {
        [SerializeField]
        private MainSceneEvent.JudgementType judgementType;

        public UniTask InvokeAsync(Actor actor, CancellationToken cancellationToken)
        {
            actor.Event.Broker.Publish(new ActorEvent.RequestGameJudgement(judgementType));
            return UniTask.CompletedTask;
        }
    }
}
