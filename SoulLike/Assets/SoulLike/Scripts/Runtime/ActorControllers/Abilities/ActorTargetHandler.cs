using System.Threading;
using R3;
using R3.Triggers;
using UnityEngine;

namespace SoulLike.ActorControllers.Abilities
{
    public sealed class ActorTargetHandler : IActorAbility
    {
        private Actor actor;

        private ActorMovement actorMovement;

        private Actor target;

        private CancellationTokenSource endLockOnTokenSource;

        public void Activate(Actor actor)
        {
            this.actor = actor;
            actorMovement = actor.GetAbility<ActorMovement>();
        }

        public void BeginLockOn(Actor target)
        {
            if (target == null)
            {
                return;
            }
            endLockOnTokenSource = new CancellationTokenSource();
            this.target = target;
            actor.UpdateAsObservable()
                .Subscribe(this, static (_, @this) =>
                {
                    @this.actorMovement.Rotate(Quaternion.LookRotation(@this.target.transform.position - @this.actor.transform.position));
                })
                .RegisterTo(endLockOnTokenSource.Token);
        }

        public void EndLockOn()
        {
            target = null;
            endLockOnTokenSource?.Cancel();
            endLockOnTokenSource?.Dispose();
            endLockOnTokenSource = null;
        }
    }
}
