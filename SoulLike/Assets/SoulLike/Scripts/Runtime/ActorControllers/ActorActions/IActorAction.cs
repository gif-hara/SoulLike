using System.Threading;
using Cysharp.Threading.Tasks;

namespace SoulLike.ActorControllers.ActorActions
{
    public interface IActorAction
    {
        UniTask InvokeAsync(Actor actor, CancellationToken cancellationToken);
    }
}
