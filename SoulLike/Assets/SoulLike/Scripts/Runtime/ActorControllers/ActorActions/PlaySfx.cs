using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using UnityEngine;

namespace SoulLike.ActorControllers.ActorActions
{
    [Serializable]
    public sealed class PlaySfx : IActorAction
    {
        [SerializeField]
        private string sfxKey;

        public UniTask InvokeAsync(Actor actor, CancellationToken cancellationToken)
        {
            TinyServiceLocator.Resolve<AudioManager>().PlaySfx(sfxKey);
            return UniTask.CompletedTask;
        }
    }
}
