using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using UnityEngine;

namespace SoulLike.ActorControllers.ActorActions
{
    [Serializable]
    public sealed class PlayBgm : IActorAction
    {
        [SerializeField]
        private string bgmKey;

        public UniTask InvokeAsync(Actor actor, CancellationToken cancellationToken)
        {
            TinyServiceLocator.Resolve<AudioManager>().PlayBgm(bgmKey);
            return UniTask.CompletedTask;
        }
    }
}
