using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using R3.Triggers;
using SoulLike.ActorControllers.Abilities;

namespace SoulLike.ActorControllers.AISystems
{
    public sealed class ActorAIController
    {
        private readonly Actor actor;

        private readonly ActorTime actorTime;

        private ActorAI currentAI;

        private CancellationToken rootScopeToken;

        private CancellationTokenSource scope;

        public float SequenceDuration { get; private set; }

        private IDisposable sequenceDurationDisposable;

        public ActorAIController(Actor actor, CancellationToken rootScopeToken)
        {
            this.actor = actor;
            actorTime = actor.GetAbility<ActorTime>();
            this.rootScopeToken = rootScopeToken;
        }

        public void Change(ActorAI actorAI)
        {
            if (actorAI == null)
            {
                currentAI = null;
                scope?.Cancel();
                scope?.Dispose();
                scope = null;
                return;
            }
            currentAI = actorAI;
            actor.Event.Broker.Receive<ActorEvent.OnBeginStun>()
                .Subscribe(this, static (_, @this) =>
                {
                    @this.BeginSequenceAsync(@this.currentAI.OnStunSequenceName).Forget();
                });
            BeginSequenceAsync(currentAI.InitialSequenceName).Forget();
        }

        public async UniTask BeginSequenceAsync(string sequenceName)
        {
            scope?.Cancel();
            scope?.Dispose();
            scope = CancellationTokenSource.CreateLinkedTokenSource(rootScopeToken);
            var sequence = currentAI.GetSequence(sequenceName);
            BeginSequenceDurationTimer();
            foreach (var action in sequence.Actions)
            {
                await action.Value.InvokeAsync(actor, this, scope.Token);
            }
        }

        private void BeginSequenceDurationTimer()
        {
            sequenceDurationDisposable?.Dispose();
            SequenceDuration = 0f;
            sequenceDurationDisposable = actor.UpdateAsObservable()
                .Subscribe(this, static (_, @this) =>
                {
                    @this.SequenceDuration += @this.actorTime.Time.deltaTime;
                });
        }
    }
}
