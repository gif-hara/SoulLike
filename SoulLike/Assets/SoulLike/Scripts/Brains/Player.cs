using System.Threading;
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

        private ActorMovement movementController;

        public Player(PlayerInput playerInput, Camera camera, PlayerSpec playerSpec)
        {
            this.playerInput = playerInput;
            this.camera = camera;
            this.playerSpec = playerSpec;
        }

        public void Attach(Actor actor, CancellationToken cancellationToken)
        {
            actor.AddAbility<ActorTime>();
            movementController = actor.AddAbility<ActorMovement>();
            movementController.SetRotationSpeed(playerSpec.RotateSpeed);
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
                    var moveVelocity = right * moveInput.x + forward * moveInput.y;
                    moveVelocity *= @this.playerSpec.MoveSpeed;
                    @this.movementController.Move(moveVelocity);

                    // 移動入力がある場合、移動方向に向く
                    if (moveVelocity.sqrMagnitude > 0.0001f)
                    {
                        var targetRotation = Quaternion.LookRotation(moveVelocity, Vector3.up);
                        @this.movementController.Rotate(targetRotation);
                    }
                })
                .RegisterTo(cancellationToken);
        }
    }
}
