using R3;
using UnityEngine;

namespace SoulLike.ActorControllers.Abilities
{
    public sealed class ActorDodge : IActorAbility
    {
        private Actor actor;

        private ActorMovement actorMovement;

        private ActorAnimation actorAnimation;

        public readonly ReactiveProperty<bool> CanDodge = new(true);

        private const string DodgeStateName = "Dodge";

        public void Activate(Actor actor)
        {
            this.actor = actor;
            actorMovement = actor.GetAbility<ActorMovement>();
            actorAnimation = actor.GetAbility<ActorAnimation>();
        }

        public bool TryDodge(Quaternion rotation)
        {
            if (!CanDodge.Value)
            {
                return false;
            }

            actorMovement.RotateImmediate(rotation);
            actorAnimation.SetTrigger(ActorAnimation.Parameter.Dodge);
            actorAnimation.UpdateAnimator();
            CanDodge.Value = false;
            actorMovement.MoveBlocker.Block(DodgeStateName);
            actorMovement.RotateBlocker.Block(DodgeStateName);
            actorAnimation.OnStateExitAsObservable()
                .Where(x => x.StateInfo.IsName(ActorAnimation.Parameter.Dodge))
                .Take(1)
                .Subscribe(this, static (_, @this) =>
                {
                    @this.CanDodge.Value = true;
                    @this.actorMovement.MoveBlocker.Unblock(DodgeStateName);
                    @this.actorMovement.RotateBlocker.Unblock(DodgeStateName);
                })
                .RegisterTo(actor.destroyCancellationToken);

            return true;
        }
    }
}
