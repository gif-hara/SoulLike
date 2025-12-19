
using SoulLike.ActorControllers.AISystems;
using SoulLike.MasterDataSystem;
using UnityEngine;

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

            public readonly int Damage;

            public readonly Vector3 hitPoint;

            public OnGiveDamage(AttackData attackData, bool isStunned, int damage, Vector3 hitPoint)
            {
                AttackData = attackData;
                IsStunned = isStunned;
                Damage = damage;
                this.hitPoint = hitPoint;
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

        public readonly struct ReviveEnemy
        {
            public readonly ActorStatusSpec ActorStatusSpec;

            public readonly IAdditionalStatus AdditionalStatus;

            public readonly ActorAI NewAI;

            public ReviveEnemy(ActorStatusSpec actorStatusSpec, IAdditionalStatus additionalStatus, ActorAI newAI)
            {
                ActorStatusSpec = actorStatusSpec;
                AdditionalStatus = additionalStatus;
                NewAI = newAI;
            }
        }

        public readonly struct RequestEffectMessage
        {
            public readonly string Message;

            public readonly Color BackgroundColor;

            public readonly Color ForwardColor;

            public RequestEffectMessage(string message, Color backgroundColor, Color forwardColor)
            {
                BackgroundColor = backgroundColor;
                ForwardColor = forwardColor;
                Message = message;
            }
        }

        public readonly struct OnCompleteEffectMessage
        {
        }
    }
}
