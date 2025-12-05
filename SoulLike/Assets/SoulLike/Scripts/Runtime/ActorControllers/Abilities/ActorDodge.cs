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

        public void Activate(Actor actor)
        {
            this.actor = actor;
            actorMovement = actor.GetAbility<ActorMovement>();
            actorAnimation = actor.GetAbility<ActorAnimation>();
        }

        public bool TryDodge(Vector3 direction)
        {
            if (!CanDodge.Value)
            {
                return false;
            }

            actorMovement.RotateImmediate(Quaternion.LookRotation(direction, Vector3.up));
            actorAnimation.SetTrigger(ActorAnimation.Parameter.Dodge);
            actorAnimation.UpdateAnimator();
            CanDodge.Value = false;
            actorAnimation.OnStateExitAsObservable()
                .Where(x => x.StateInfo.IsName(ActorAnimation.Parameter.Dodge))
                .Take(1)
                .Subscribe(this, static (_, @this) =>
                {
                    @this.CanDodge.Value = true;
                })
                .RegisterTo(actor.destroyCancellationToken);

            return true;
        }
    }
}
