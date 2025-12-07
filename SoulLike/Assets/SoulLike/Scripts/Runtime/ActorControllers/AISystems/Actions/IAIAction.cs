using System.Threading;
using Cysharp.Threading.Tasks;

namespace SoulLike.ActorControllers.AISystems
{
    public interface IAIAction
    {
        UniTask InvokeAsync(Actor actor, ActorAIController actorAIController, CancellationToken cancellationToken);
    }
}
