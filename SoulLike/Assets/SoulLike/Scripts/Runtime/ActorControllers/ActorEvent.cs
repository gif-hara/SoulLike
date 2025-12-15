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
            public readonly AttackData AttackData;

            public readonly bool IsStunned;

            public OnGiveDamage(AttackData attackData, bool isStunned)
            {
                AttackData = attackData;
                IsStunned = isStunned;
            }
        }

        public readonly struct OnBeginStun
        {
        }

        public readonly struct OnRecoveryHitPoint
        {
        }

        public readonly struct OnSetAttackBuffTimer
        {
        }
    }
}
