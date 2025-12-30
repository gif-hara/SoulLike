using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using SoulLike.ActorControllers.Abilities;
using SoulLike.ActorControllers.AISystems;
using SoulLike.MasterDataSystem;
using UnityEngine;

namespace SoulLike.ActorControllers.Brains
{
    public sealed class Enemy : IActorBrain
    {
        private readonly EnemySpec enemySpec;

        private readonly MessageBroker sceneBroker;

        private readonly Actor target;

        private readonly UIViewEffectMessage uiViewEffectMessage;

        private Vector3 initialPosition;

        private Quaternion initialRotation;

        private ActorMovement actorMovement;

        private ActorStatus actorStatus;

        private ActorTargetHandler actorTargetHandler;

        private ActorAIController actorAIController;

        private ActorAnimation actorAnimation;

        private ActorWeaponHandler actorWeaponHandler;

        private ActorEffect actorEffect;

        private int deadCount = 0;

        public Enemy(EnemySpec enemySpec, MessageBroker sceneBroker, Actor target, UIViewEffectMessage uiViewEffectMessage)
        {
            this.enemySpec = enemySpec;
            this.sceneBroker = sceneBroker;
            this.target = target;
            this.uiViewEffectMessage = uiViewEffectMessage;
        }

        public void Attach(Actor actor, CancellationToken cancellationToken)
        {
            initialPosition = actor.transform.position;
            initialRotation = actor.transform.rotation;
            actor.gameObject.SetLayerRecursive(Layer.Enemy);
            actor.AddAbility<ActorTime>();
            actorMovement = actor.AddAbility<ActorMovement>();
            actor.AddAbility<ActorSceneViewHandler>();
            actorAnimation = actor.AddAbility<ActorAnimation>();
            actorTargetHandler = actor.AddAbility<ActorTargetHandler>();
            var actorWalk = actor.AddAbility<ActorWalk>();
            actorWeaponHandler = actor.AddAbility<ActorWeaponHandler>();
            actor.AddAbility<ActorDodge>();
            actorStatus = actor.AddAbility<ActorStatus>();
            actorEffect = actor.AddAbility<ActorEffect>();
            actor.ActivateAbilities(cancellationToken);

            actorWeaponHandler.CreateWeapon(enemySpec.WeaponPrefab, Layer.EnemyWeapon);
            actorWalk.MoveSpeed = enemySpec.MoveSpeed;
            actorWalk.Acceleration = enemySpec.MoveAcceleration;
            actorStatus.ApplySpec(enemySpec.ActorStatusSpec, new AdditionalStatusEmpty());
            actorAIController = new ActorAIController(actor, cancellationToken);
            actorAIController.Change(enemySpec.ActorAI);
            actorTargetHandler.BeginLockOn(target);
            actorAnimation.PlayAnimation(ActorAnimation.Parameter.Idle);

            sceneBroker.Receive<MainSceneEvent.RestartGame>()
                .Subscribe((this, actor), static (x, t) =>
                {
                    var (@this, actor) = t;
                    foreach (var action in @this.enemySpec.OnRestartActions)
                    {
                        action.InvokeAsync(actor, actor.destroyCancellationToken).Forget();
                    }
                    @this.deadCount = 0;
                    @this.actorEffect.Reset();
                    @this.actorMovement.Reset();
                    @this.actorWeaponHandler.Reset();
                    @this.actorAnimation.Reset();
                    @this.actorStatus.ApplySpec(@this.enemySpec.ActorStatusSpec, new AdditionalStatusEmpty());
                    @this.actorMovement.Teleport(@this.initialPosition, @this.initialRotation);
                    @this.actorTargetHandler.BeginLockOn(@this.target);
                    @this.actorAIController.Change(@this.enemySpec.ActorAI);
                })
                .RegisterTo(cancellationToken);

            actor.Event.Broker.Receive<ActorEvent.OnDead>()
                .Subscribe((this, actor), static (x, t) =>
                {
                    var (@this, actor) = t;
                    @this.enemySpec.OnDeadActions[@this.deadCount].InvokeAsync(actor, actor.destroyCancellationToken).Forget();
                    @this.deadCount = Mathf.Min(@this.deadCount + 1, @this.enemySpec.OnDeadActions.Count - 1);
                })
                .RegisterTo(cancellationToken);

            actor.Event.Broker.Receive<ActorEvent.ReviveEnemy>()
                .SubscribeAwait((this, actor), async static (x, t, cts) =>
                {
                    var (@this, actor) = t;
                    @this.actorStatus.ApplySpec(x.ActorStatusSpec, x.AdditionalStatus);
                    @this.actorMovement.Reset();
                    @this.actorWeaponHandler.Reset();
                    await @this.actorAnimation.OnStateEnterAsObservable(ActorAnimation.Parameter.Idle).FirstAsync(cts).AsUniTask();
                    @this.actorTargetHandler.BeginLockOn(@this.target);
                    @this.actorAnimation.Reset();
                    @this.actorAIController.Change(x.NewAI);
                })
                .RegisterTo(cancellationToken);

            actor.Event.Broker.Receive<ActorEvent.RequestEffectMessage>()
                .Subscribe((this, actor), static (x, t) =>
                {
                    var (@this, actor) = t;
                    @this.uiViewEffectMessage.BeginAsync(x.BackgroundColor, x.ForwardColor, x.MessageColor, x.Message, actor.Event.Broker, actor.destroyCancellationToken).Forget();
                })
                .RegisterTo(cancellationToken);

            actor.Event.Broker.Receive<ActorEvent.ChangeAI>()
                .Subscribe((this, actor), static (x, t) =>
                {
                    var (@this, actor) = t;
                    @this.actorAIController.Change(x.NewAI);
                })
                .RegisterTo(cancellationToken);

            actor.Event.Broker.Receive<ActorEvent.RequestGameJudgement>()
                .Subscribe((this, actor), static (x, t) =>
                {
                    var (@this, actor) = t;
                    @this.sceneBroker.Publish(new MainSceneEvent.GameJudgement(x.JudgementType));
                })
                .RegisterTo(cancellationToken);

            actor.Event.Broker.Receive<ActorEvent.RequestBeginEvent>()
                .Subscribe(sceneBroker, static (x, broker) =>
                {
                    broker.Publish(new MainSceneEvent.OnBeginEvent(x.Tag));
                })
                .RegisterTo(cancellationToken);

            actor.Event.Broker.Receive<ActorEvent.RequestEndEvent>()
                .Subscribe(sceneBroker, static (x, broker) =>
                {
                    broker.Publish(new MainSceneEvent.OnEndEvent(x.Tag));
                })
                .RegisterTo(cancellationToken);
        }
    }
}
