using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SoulLike.ActorControllers.Abilities;
using UnityEngine;

namespace SoulLike.ActorControllers.ActorActions
{
    [Serializable]
    public sealed class SetMaterial : IActorAction
    {
        [SerializeField]
        private Material[] materials;

        public UniTask InvokeAsync(Actor actor, CancellationToken cancellationToken)
        {
            foreach (var renderer in actor.GetAbility<ActorSceneViewHandler>().SceneView.Renderers)
            {
                renderer.materials = materials;
            }
            return UniTask.CompletedTask;
        }
    }
}
