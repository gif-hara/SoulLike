using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SoulLike.ActorControllers.AISystems
{
    [Serializable]
    public sealed class Log : IAIAction
    {
        [SerializeField]
        private string message;

        public UniTask InvokeAsync(Actor actor, CancellationToken cancellationToken)
        {
            Debug.Log(message);
            return UniTask.CompletedTask;
        }
    }
}
