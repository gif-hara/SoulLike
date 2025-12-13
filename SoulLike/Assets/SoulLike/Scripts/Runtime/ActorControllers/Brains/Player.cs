using System;
using System.Threading;
using HK;
using R3;
using R3.Triggers;
using SoulLike.ActorControllers.Abilities;
using SoulLike.MasterDataSystem;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SoulLike.ActorControllers.Brains
{
    public sealed class Player : IActorBrain
    {
        private readonly PlayerInput playerInput;

        private readonly Camera camera;

        private readonly PlayerSpec playerSpec;

        private readonly UserData userData;

        private readonly MessageBroker sceneBroker;

        private Vector3 initialPosition;

        private Quaternion initialRotation;

        private ActorMovement actorMovement;

        private ActorWeaponHandler actorWeaponHandler;

        private ActorDodge actorDodge;

        private ActorWalk actorWalk;

        private ActorStatus actorStatus;

        private ActorAnimation actorAnimation;

        private ActorTargetHandler actorTargetHandler;

        private IDisposable preInputProcessDisposable;

        private Vector3 lastMoveInput;

        public Player(PlayerInput playerInput, Camera camera, PlayerSpec playerSpec, UserData userData, MessageBroker sceneBroker)
        {
            this.playerInput = playerInput;
            this.camera = camera;
            this.playerSpec = playerSpec;
            this.userData = userData;
            this.sceneBroker = sceneBroker;
        }

        public void Attach(Actor actor, CancellationToken cancellationToken)
        {
            initialPosition = actor.transform.position;
            initialRotation = actor.transform.rotation;
            actor.gameObject.SetLayerRecursive(Layer.Player);
            actor.AddAbility<ActorTime>();
            actorMovement = actor.AddAbility<ActorMovement>();
            actor.AddAbility<ActorSceneViewHandler>();
            actorAnimation = actor.AddAbility<ActorAnimation>();
            actorWeaponHandler = actor.AddAbility<ActorWeaponHandler>();
            actorDodge = actor.AddAbility<ActorDodge>();
            actorStatus = actor.AddAbility<ActorStatus>();
            actorTargetHandler = actor.AddAbility<ActorTargetHandler>();
            actorWalk = actor.AddAbility<ActorWalk>();
            actor.ActivateAbilities();

            actorWeaponHandler.CreateWeapon(playerSpec.WeaponPrefab, Layer.PlayerWeapon);
            actorStatus.ApplySpec(playerSpec.ActorStatusSpec, new AdditionalStatusEmpty());
            actorWalk.MoveSpeed = playerSpec.MoveSpeed;
            actorDodge.DodgeStaminaCost = playerSpec.ActorStatusSpec.DodgeStaminaCost;

            actorMovement.SetRotationSpeed(playerSpec.RotateSpeed);
            actor.UpdateAsObservable()
                .Subscribe((this, actor), static (_, t) =>
                {
                    var (@this, actor) = t;
                    var moveInput = @this.playerInput.actions["Move"].ReadValue<Vector2>();
                    var camTransform = @this.camera.transform;
                    var forward = camTransform.forward;
                    var right = camTransform.right;
                    forward.y = 0;
                    right.y = 0;
                    forward.Normalize();
                    right.Normalize();
                    @this.lastMoveInput = right * moveInput.x + forward * moveInput.y;
                    @this.actorWalk.SetNormalizedVelocity(@this.lastMoveInput);
                })
                .RegisterTo(cancellationToken);
            playerInput.actions["Attack"].OnPerformedAsObservable()
                .Subscribe((this, actor), static (_, t) =>
                {
                    var (@this, actor) = t;
                    @this.PreInputProcess(actor, () => @this.actorWeaponHandler.TryBasicAttack());
                })
                .RegisterTo(cancellationToken);
            playerInput.actions["Dodge"].OnPerformedAsObservable()
                .Subscribe((this, actor), static (_, t) =>
                {
                    var (@this, actor) = t;
                    var rotation = @this.lastMoveInput.sqrMagnitude > 0.01f
                        ? Quaternion.LookRotation(@this.lastMoveInput, Vector3.up)
                        : actor.transform.rotation;
                    @this.PreInputProcess(actor, () => @this.actorDodge.TryDodge(rotation));
                })
                .RegisterTo(cancellationToken);
            playerInput.actions["Parry"].OnPerformedAsObservable()
                .Subscribe((this, actor), static (_, t) =>
                {
                    var (@this, actor) = t;
                    @this.PreInputProcess(actor, () => @this.actorWeaponHandler.TryUniqueAttack(0));
                })
                .RegisterTo(cancellationToken);
            actor.Event.Broker.Receive<ActorEvent.OnGiveDamage>()
                .Subscribe(this, static (x, @this) =>
                {
                    var attackData = x.AttackData;
                    @this.userData.AddExperience(attackData.EarnedExperience);
                })
                .RegisterTo(cancellationToken);
            actor.Event.Broker.Receive<ActorEvent.OnDead>()
                .Subscribe((this), static (_, @this) =>
                {
                    @this.sceneBroker.Publish(new MainSceneEvent.GameJudgement(MainSceneEvent.JudgementType.PlayerLose));
                })
                .RegisterTo(cancellationToken);
            sceneBroker.Receive<MainSceneEvent.RestartGame>()
                .Subscribe(this, static (x, @this) =>
                {
                    @this.actorAnimation.Reset();
                    @this.actorStatus.ApplySpec(@this.playerSpec.ActorStatusSpec, new AdditionalStatusEmpty());
                    @this.actorMovement.Teleport(@this.initialPosition, @this.initialRotation);
                    @this.actorTargetHandler.BeginLockOn(x.Enemy);
                })
                .RegisterTo(cancellationToken);
        }

        private void PreInputProcess(Actor actor, Func<bool> process)
        {
            preInputProcessDisposable?.Dispose();
            preInputProcessDisposable = actor.UpdateAsObservable()
                .Take(TimeSpan.FromSeconds(0.5f))
                .TakeWhile(_ => !process())
                .Subscribe();
        }
    }
}
