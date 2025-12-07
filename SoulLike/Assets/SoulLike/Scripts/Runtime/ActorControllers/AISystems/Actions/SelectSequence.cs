using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using SoulLike.ActorControllers.AISystems.SelectSequences;
using TNRD;
using UnityEngine;

namespace SoulLike.ActorControllers.AISystems.Actions
{
    public class SelectSequence : IAIAction
    {
        [SerializeField]
        private List<SerializableInterface<ISelectSequence>> selectors = new();

        public UniTask InvokeAsync(Actor actor, ActorAIController actorAIController, CancellationToken cancellationToken)
        {
            foreach (var selector in selectors)
            {
                var sequenceName = selector.Value.Select(actor, actorAIController);
                if (!string.IsNullOrEmpty(sequenceName))
                {
                    actorAIController.BeginSequenceAsync(sequenceName).Forget();
                    break;
                }
            }

            return UniTask.CompletedTask;
        }
    }
}
