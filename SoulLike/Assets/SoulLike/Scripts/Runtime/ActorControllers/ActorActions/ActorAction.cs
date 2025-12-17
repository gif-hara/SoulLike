using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TNRD;
using UnityEngine;

namespace SoulLike.ActorControllers.ActorActions
{
    [CreateAssetMenu(menuName = "SoulLike/ActorControllers/ActorAction")]
    public class ActorAction : ScriptableObject
    {
#if UNITY_EDITOR
        [ClassesOnly]
#endif
        [SerializeField]
        private List<SerializableInterface<IActorAction>> actions = new();

        public async UniTask InvokeAsync(Actor actor, CancellationToken cancellationToken)
        {
            foreach (var action in actions)
            {
                await action.Value.InvokeAsync(actor, cancellationToken);
            }
        }
    }
}
