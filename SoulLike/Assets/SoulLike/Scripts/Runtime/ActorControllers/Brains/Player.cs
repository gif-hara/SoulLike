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

        private readonly Camera camera;

        private readonly PlayerSpec playerSpec;

        private ActorMovement actorMovement;

        private ActorAnimation actorAnimation;

        private ActorWeaponHandler actorWeaponHandler;

        private ActorDodge actorDodge;

        private ActorTargetHandler actorTargetHandler;

        private ActorWalk actorWalk;

        private IDisposable preInputProcessDisposable;

        private Vector3 lastMoveInput;

        public Player(PlayerInput playerInput, Camera camera, PlayerSpec playerSpec)
        {
            this.playerInput = playerInput;
            this.camera = camera;
            this.playerSpec = playerSpec;
        }

        public void Attach(Actor actor, CancellationToken cancellationToken)
        {
            actor.gameObject.SetLayerRecursive(Layer.Player);
            actor.AddAbility<ActorTime>();
            actorMovement = actor.AddAbility<ActorMovement>();
            actor.AddAbility<ActorSceneViewHandler>();
            actorAnimation = actor.AddAbility<ActorAnimation>();
            actorWeaponHandler = actor.AddAbility<ActorWeaponHandler>();
            actorDodge = actor.AddAbility<ActorDodge>();
            var actorStatus = actor.AddAbility<ActorStatus>();
            actorTargetHandler = actor.AddAbility<ActorTargetHandler>();
            actorWalk = actor.AddAbility<ActorWalk>();
            actor.AddAbility<ActorInvincible>();
            actor.ActivateAbilities();

            actorWeaponHandler.CreateWeapon(playerSpec.WeaponPrefab, Layer.PlayerWeapon);
            actorStatus.ApplySpec(playerSpec.ActorStatusSpec);
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
                    @this.PreInputProcess(actor, () => @this.actorDodge.TryDodge(Quaternion.LookRotation(@this.lastMoveInput, Vector3.up)));
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
