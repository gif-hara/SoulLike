using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using R3.Triggers;
using SoulLike.ActorControllers.Abilities;
using SoulLike.MasterDataSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SoulLike.ActorControllers.Brains
{
    public sealed class Player : IActorBrain
    {
        private readonly PlayerInput playerInput;

        private readonly WorldCameraController worldCameraController;

        private readonly Camera camera;

        private readonly PlayerSpec playerSpec;

        private readonly UserData userData;

        private readonly MessageBroker sceneBroker;

        private readonly Actor target;

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

        public Player(PlayerInput playerInput, WorldCameraController worldCameraController, PlayerSpec playerSpec, UserData userData, MessageBroker sceneBroker, Actor target)
        {
            this.playerInput = playerInput;
            this.worldCameraController = worldCameraController;
            camera = worldCameraController.WorldCamera;
            this.playerSpec = playerSpec;
            this.userData = userData;
            this.sceneBroker = sceneBroker;
            this.target = target;
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
            actor.ActivateAbilities(cancellationToken);

            actorWeaponHandler.CreateWeapon(playerSpec.WeaponPrefab, Layer.PlayerWeapon);
            actorStatus.ApplySpec(playerSpec.ActorStatusSpec, userData);
            actorWalk.MoveSpeed = playerSpec.MoveSpeed;
            actorWalk.Acceleration = playerSpec.MoveAcceleration;
            actorDodge.DodgeStaminaCost = playerSpec.ActorStatusSpec.DodgeStaminaCost;
            actorTargetHandler.BeginLockOn(target);

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
            playerInput.actions["StrongAttack"].OnPerformedAsObservable()
                .Subscribe((this, actor), static (_, t) =>
                {
                    var (@this, actor) = t;
                    var uniqueAttackId = @this.playerSpec.StrongAttackUniqueAttackIds[@this.userData.StrongAttackUniqueAttackId];
                    @this.PreInputProcess(actor, () => @this.actorWeaponHandler.TryUniqueAttack(uniqueAttackId));
                })
                .RegisterTo(cancellationToken);
            playerInput.actions["Dodge"].OnPerformedAsObservable()
                .Subscribe((this, actor), static (_, t) =>
                {
                    var (@this, actor) = t;
                    @this.PreInputProcess(actor, () =>
                    {
                        if (@this.userData.DodgeUniqueAttackId != 0)
                        {
                            return @this.actorWeaponHandler.TryUniqueAttack(@this.userData.DodgeUniqueAttackId);
                        }
                        else
                        {
                            var rotation = @this.lastMoveInput.sqrMagnitude > 0.01f
                                ? Quaternion.LookRotation(@this.lastMoveInput, Vector3.up)
                                : actor.transform.rotation;
                            return @this.actorDodge.TryDodge(rotation);
                        }
                    });
                })
                .RegisterTo(cancellationToken);
            playerInput.actions["Parry"].OnPerformedAsObservable()
                .Subscribe((this, actor), static (_, t) =>
                {
                    var (@this, actor) = t;
                    var uniqueAttackId = @this.playerSpec.ParryUniqueAttackIds[@this.userData.ParryUniqueAttackId];
                    @this.PreInputProcess(actor, () => @this.actorWeaponHandler.TryUniqueAttack(uniqueAttackId));
                })
                .RegisterTo(cancellationToken);
            actor.Event.Broker.Receive<ActorEvent.OnGiveDamage>()
                .Subscribe(this, static (x, @this) =>
                {
                    var attackData = x.AttackData;
                    @this.userData.AddExperience((int)(attackData.EarnedExperience * @this.userData.AqcuireExperienceRate.CurrentValue * x.Target.GetAbility<ActorStatus>().ExperienceRate));
                })
                .RegisterTo(cancellationToken);
            actor.Event.Broker.Receive<ActorEvent.OnDead>()
                .SubscribeAwait((this, actor), async static (_, t, cts) =>
                {
                    var (@this, actor) = t;
                    @this.userData.DeadCount++;
                    @this.worldCameraController.PlayOnDeadImpulse1();
                    var audioManager = TinyServiceLocator.Resolve<AudioManager>();
                    audioManager.PlaySfx("BeginStun.1");
                    audioManager.FadeOutBgmAsync(0.0f, 0.5f, cts).Forget();
                    await HK.Time.Root.BeginHitStopAsync(1.0f, 0.0f, cts);
                    @this.worldCameraController.PlayLockOnCameraNoiseAnimationAsync(10.0f, 0.0f, 3.0f, cts).Forget();
                    audioManager.PlaySfx("Defeat.1");
                    audioManager.FadeOutBgmAsync(2.0f, 0.0f, cts).Forget();
                    await HK.Time.Root.BeginHitStopAsync(1.0f, 0.3f, cts);
                    @this.sceneBroker.Publish(new MainSceneEvent.GameJudgement(MainSceneEvent.JudgementType.PlayerLose));
                })
                .RegisterTo(cancellationToken);
            sceneBroker.Receive<MainSceneEvent.RestartGame>()
                .Subscribe(this, static (x, @this) =>
                {
                    @this.actorMovement.Reset();
                    @this.actorWeaponHandler.Reset();
                    @this.actorDodge.Reset();
                    @this.actorAnimation.Reset();
                    @this.actorStatus.ApplySpec(@this.playerSpec.ActorStatusSpec, @this.userData);
                    @this.actorMovement.Teleport(@this.initialPosition, @this.initialRotation);
                    @this.actorTargetHandler.BeginLockOn(@this.target);
                })
                .RegisterTo(cancellationToken);
        }

        private void PreInputProcess(Actor actor, Func<bool> process)
        {
            preInputProcessDisposable?.Dispose();
            if (actorStatus.IsDead)
            {
                preInputProcessDisposable = null;
                return;
            }
            preInputProcessDisposable = actor.UpdateAsObservable()
                .Take(TimeSpan.FromSeconds(0.5f))
                .TakeWhile(_ => !process())
                .Subscribe();
        }
    }
}
