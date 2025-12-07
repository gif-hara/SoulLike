using R3;

namespace SoulLike.ActorControllers.Abilities
{
    public class ActorInvincible : IActorAbility
    {
        private Actor actor;

        private Blocker invincibleBlocker = new();

        private ReactiveProperty<bool> isInvincible = new(false);

        public ReadOnlyReactiveProperty<bool> IsInvincible => isInvincible;

        public void Activate(Actor actor)
        {
            this.actor = actor;
        }

        public void BeginInvincible(string topic)
        {
            invincibleBlocker.Block(topic);
            isInvincible.Value = true;
        }

        public void EndInvincible(string topic)
        {
            invincibleBlocker.Unblock(topic);
            if (!invincibleBlocker.IsBlocked)
            {
                isInvincible.Value = false;
            }
        }
    }
}
