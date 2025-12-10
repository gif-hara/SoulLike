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

        private ActorAnimation actorAnimation;

        public Actor Target { get; private set; }

        private CancellationTokenSource endLockOnTokenSource;

        public void Activate(Actor actor)
        {
            this.actor = actor;
            actorMovement = actor.GetAbility<ActorMovement>();
            actorAnimation = actor.GetAbility<ActorAnimation>();
            actor.Event.Broker.Receive<ActorEvent.OnDead>()
                .Subscribe(this, static (_, @this) => @this.EndLockOn())
                .RegisterTo(actor.destroyCancellationToken);
        }

        public void BeginLockOn(Actor target)
        {
            if (target == null)
            {
                return;
            }
            endLockOnTokenSource = new CancellationTokenSource();
            this.Target = target;
            actorAnimation.SetBool(ActorAnimation.Parameter.LockedOn, true);
            actor.UpdateAsObservable()
                .Subscribe(this, static (_, @this) =>
                {
                    @this.actorMovement.Rotate(Quaternion.LookRotation(@this.Target.transform.position - @this.actor.transform.position));
                })
                .RegisterTo(endLockOnTokenSource.Token);
        }

        public void EndLockOn()
        {
            Target = null;
            endLockOnTokenSource?.Cancel();
            endLockOnTokenSource?.Dispose();
            endLockOnTokenSource = null;
            actorAnimation.SetBool(ActorAnimation.Parameter.LockedOn, false);
        }

        public float GetDistanceToTarget()
        {
            if (Target == null)
            {
                return float.MaxValue;
            }
            return Vector3.Distance(actor.transform.position, Target.transform.position);
        }
    }
}
