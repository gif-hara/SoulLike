using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace SoulLike.ActorControllers.ActorActions
{
    [Serializable]
    public sealed class BeginEffectMessage : IActorAction
    {
        [SerializeField, Multiline]
        private string message;

        [SerializeField]
        private Color backgroundColor;

        [SerializeField]
        private Color forwardColor;

        public async UniTask InvokeAsync(Actor actor, CancellationToken cancellationToken)
        {
            actor.Event.Broker.Publish(new ActorEvent.RequestEffectMessage(message, backgroundColor, forwardColor));
            await actor.Event.Broker.Receive<ActorEvent.OnCompleteEffectMessage>().FirstAsync(cancellationToken).AsUniTask();
        }
    }
}
