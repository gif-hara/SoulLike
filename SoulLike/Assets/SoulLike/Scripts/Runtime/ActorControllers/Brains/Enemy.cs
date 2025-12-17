using System.Threading;
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

        private Vector3 initialPosition;

        private Quaternion initialRotation;

        private ActorMovement actorMovement;

        private ActorStatus actorStatus;

        private ActorTargetHandler actorTargetHandler;

        private ActorAIController actorAIController;

        private ActorAnimation actorAnimation;

        private ActorWeaponHandler actorWeaponHandler;

        public Enemy(EnemySpec enemySpec, MessageBroker sceneBroker)
        {
            this.enemySpec = enemySpec;
            this.sceneBroker = sceneBroker;
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
            actor.ActivateAbilities();

            actorWeaponHandler.CreateWeapon(enemySpec.WeaponPrefab, Layer.EnemyWeapon);
            actorWalk.MoveSpeed = enemySpec.MoveSpeed;
            actorWalk.Acceleration = enemySpec.MoveAcceleration;
            actorStatus.ApplySpec(enemySpec.ActorStatusSpec, new AdditionalStatusEmpty());
            actorAIController = new ActorAIController(actor);
            actorAIController.Change(enemySpec.ActorAI);

            sceneBroker.Receive<MainSceneEvent.RestartGame>()
                .Subscribe(this, static (x, @this) =>
                {
                    @this.actorMovement.Reset();
                    @this.actorWeaponHandler.Reset();
                    @this.actorAnimation.Reset();
                    @this.actorStatus.ApplySpec(@this.enemySpec.ActorStatusSpec, new AdditionalStatusEmpty());
                    @this.actorMovement.Teleport(@this.initialPosition, @this.initialRotation);
                    @this.actorTargetHandler.BeginLockOn(x.Player);
                    @this.actorAIController.Change(@this.enemySpec.ActorAI);
                })
                .RegisterTo(cancellationToken);
        }
    }
}
