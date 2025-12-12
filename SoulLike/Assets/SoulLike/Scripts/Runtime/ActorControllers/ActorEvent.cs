namespace SoulLike.ActorControllers
{
    public sealed class ActorEvent
    {
        private readonly MessageBroker messageBroker = new();

        public IMessageBroker Broker => messageBroker;

        public readonly struct BeginAttack
        {
            public readonly int AttackId;

            public BeginAttack(int attackId)
            {
                AttackId = attackId;
            }
        }

        public readonly struct EndAttack
        {
            public readonly int AttackId;

            public EndAttack(int attackId)
            {
                AttackId = attackId;
            }
        }

        public readonly struct OnDead
        {
        }

        public readonly struct OnBeginParry
        {
        }

        public readonly struct OnGiveDamage
        {
            public readonly bool IsStunned;

            public OnGiveDamage(bool isStunned)
            {
                IsStunned = isStunned;
            }
        }
    }
}
