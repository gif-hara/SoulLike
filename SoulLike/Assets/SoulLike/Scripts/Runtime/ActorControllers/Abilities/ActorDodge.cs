using R3;
using UnityEngine;

namespace SoulLike.ActorControllers.Abilities
{
    public sealed class ActorDodge : IActorAbility
    {
        private Actor actor;

        private ActorMovement actorMovement;

        private ActorAnimation actorAnimation;

        private ActorStatus actorStatus;

        public readonly Blocker DodgeBlocker = new();

        public readonly ReactiveProperty<bool> CanDodge = new(true);

        public float DodgeStaminaCost { get; set; }

        private const string DodgeStateName = "Dodge";

        public void Reset()
        {
            DodgeBlocker.Reset();
        }

        public void Activate(Actor actor)
        {
            this.actor = actor;
            actorMovement = actor.GetAbility<ActorMovement>();
            actorAnimation = actor.GetAbility<ActorAnimation>();
            actorStatus = actor.GetAbility<ActorStatus>();
        }

        public bool TryDodge(Quaternion rotation)
        {
            if (DodgeBlocker.IsBlocked)
            {
                return false;
            }

            if (!actorStatus.CanUseStamina())
            {
                return false;
            }

            actorStatus.UseStamina(DodgeStaminaCost);
            actorMovement.RotateImmediate(rotation);
            actorAnimation.SetTrigger(ActorAnimation.Parameter.Dodge);
            actorAnimation.UpdateAnimator();
            DodgeBlocker.Block(DodgeStateName);
            actorMovement.MoveBlocker.Block(DodgeStateName);
            actorMovement.RotateBlocker.Block(DodgeStateName);
            actorStatus.BeginInvincible(DodgeStateName);
            actorStatus.StaminaRecoveryBlocker.Block(DodgeStateName);
            actorAnimation.OnStateExitAsObservable(ActorAnimation.Parameter.Dodge)
                .Take(1)
                .Subscribe(this, static (_, @this) =>
                {
                    @this.DodgeBlocker.Unblock(DodgeStateName);
                    @this.actorMovement.MoveBlocker.Unblock(DodgeStateName);
                    @this.actorMovement.RotateBlocker.Unblock(DodgeStateName);
                    @this.actorStatus.EndInvincible(DodgeStateName);
                    @this.actorStatus.StaminaRecoveryBlocker.Unblock(DodgeStateName);
                })
                .RegisterTo(actor.destroyCancellationToken);

            return true;
        }
    }
}
