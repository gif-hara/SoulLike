using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.Assertions;

namespace SoulLike.ActorControllers.Abilities
{
    public class ActorAnimationController
    {
        private readonly Actor actor;

        public ActorAnimationController(Actor actor)
        {
            this.actor = actor;
            // actor.TimeController.UpdatedTimeScale
            //     .Subscribe((actor, simpleAnimation), static (_, t) =>
            //     {
            //         var (actor, simpleAnimation) = t;
            //         foreach (var state in simpleAnimation.GetStates())
            //         {
            //             state.speed = actor.TimeController.Time.totalTimeScale;
            //         }
            //     })
            //     .RegisterTo(actor.destroyCancellationToken);
        }
    }
}
