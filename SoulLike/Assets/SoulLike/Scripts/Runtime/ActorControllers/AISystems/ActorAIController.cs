using System.Threading;
using Cysharp.Threading.Tasks;

namespace SoulLike.ActorControllers.AISystems
{
    public sealed class ActorAIController
    {
        private readonly Actor actor;

        private ActorAI currentAI;

        private CancellationTokenSource scope;

        public ActorAIController(Actor actor)
        {
            this.actor = actor;
        }

        public void Change(ActorAI actorAI)
        {
            scope?.Cancel();
            scope?.Dispose();
            scope = null;
            if (actorAI == null)
            {
                currentAI = null;
                return;
            }
            scope = CancellationTokenSource.CreateLinkedTokenSource(actor.destroyCancellationToken);
            currentAI = actorAI;
            BeginSequenceAsync(currentAI.InitialSequenceName).Forget();
        }

        public async UniTask BeginSequenceAsync(string sequenceName)
        {
            var sequence = currentAI.GetSequence(sequenceName);
            foreach (var action in sequence.Actions)
            {
                await action.Value.InvokeAsync(actor, scope.Token);
            }
        }
    }
}
