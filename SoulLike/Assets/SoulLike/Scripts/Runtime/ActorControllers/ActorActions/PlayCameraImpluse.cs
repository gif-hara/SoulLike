using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using UnityEngine;

namespace SoulLike.ActorControllers.ActorActions
{
    [Serializable]
    public sealed class PlayCameraImpulse : IActorAction
    {
        [SerializeField]
        private WorldCameraController.ImpluseType impluseType;

        public UniTask InvokeAsync(Actor actor, CancellationToken cancellationToken)
        {
            var worldCameraController = TinyServiceLocator.Resolve<WorldCameraController>();
            worldCameraController.PlayImpulse(impluseType);
            return UniTask.CompletedTask;
        }
    }
}
