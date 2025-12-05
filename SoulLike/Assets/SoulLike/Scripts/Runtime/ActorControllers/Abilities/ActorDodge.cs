using R3;
using SoulLike.ActorControllers.Brains;

namespace SoulLike.ActorControllers.Abilities
{
    public sealed class ActorDodge : IActorAbility
    {
        private Actor actor;

        private ActorAnimation actorAnimation;

        public readonly ReactiveProperty<bool> CanDodge = new(true);

        public void Activate(Actor actor)
        {
            this.actor = actor;
            actorAnimation = actor.GetAbility<ActorAnimation>();
        }

        public bool TryDodge()
        {
            if (!CanDodge.Value)
            {
                return false;
            }

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
