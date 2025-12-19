using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SoulLike.ActorControllers.Abilities;
using UnityEngine;

namespace SoulLike.ActorControllers.ActorActions
{
    [Serializable]
    public sealed class Instantiate : IActorAction
    {
        [SerializeField]
        private GameObject prefab;

        [SerializeField]
        private string locatorName;

        public UniTask InvokeAsync(Actor actor, CancellationToken cancellationToken)
        {
            var instance = UnityEngine.Object.Instantiate(prefab);
            var locator = actor.GetAbility<ActorSceneViewHandler>().SceneView.LocatorHolder.Get(locatorName);
            instance.transform.SetPositionAndRotation(locator.position, locator.rotation);
            return UniTask.CompletedTask;
        }
    }
}
