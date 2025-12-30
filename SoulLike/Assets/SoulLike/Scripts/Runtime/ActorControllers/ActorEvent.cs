
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

            public readonly Vector3 HitPoint;

            public readonly Actor Target;

            public OnGiveDamage(AttackData attackData, bool isStunned, int damage, Vector3 hitPoint, Actor target)
            {
                AttackData = attackData;
                IsStunned = isStunned;
                Damage = damage;
                HitPoint = hitPoint;
                Target = target;
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

            public readonly Color MessageColor;

            public RequestEffectMessage(string message, Color backgroundColor, Color forwardColor, Color messageColor)
            {
                BackgroundColor = backgroundColor;
                ForwardColor = forwardColor;
                MessageColor = messageColor;
                Message = message;
            }
        }

        public readonly struct OnCompleteEffectMessage
        {
        }

        public readonly struct ChangeAI
        {
            public readonly ActorAI NewAI;

            public ChangeAI(ActorAI newAI)
            {
                NewAI = newAI;
            }
        }

        public readonly struct RequestGameJudgement
        {
            public readonly MainSceneEvent.JudgementType JudgementType;

            public RequestGameJudgement(MainSceneEvent.JudgementType judgementType)
            {
                JudgementType = judgementType;
            }
        }

        public readonly struct RequestBeginEvent
        {
            public readonly string Tag;

            public RequestBeginEvent(string tag)
            {
                Tag = tag;
            }
        }

        public readonly struct RequestEndEvent
        {
            public readonly string Tag;

            public RequestEndEvent(string tag)
            {
                Tag = tag;
            }
        }
    }
}
