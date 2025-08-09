using System.Threading;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SoulLike.ActorControllers.Brains
{
    public sealed class Player : IActorBrain
    {
        private readonly PlayerInput playerInput;
        private readonly Camera camera;

        public Player(PlayerInput playerInput, Camera camera)
        {
            this.playerInput = playerInput;
            this.camera = camera;
        }

        public void Attach(Actor actor, CancellationToken cancellationToken)
        {
            actor.UpdateAsObservable()
                .Subscribe((this, actor), static (_, t) =>
                {
                    var (@this, actor) = t;
                    var moveInput = @this.playerInput.actions["Move"].ReadValue<Vector2>();
                    // カメラの方向に合わせて移動ベクトルを変換
                    var camTransform = @this.camera.transform;
                    var forward = camTransform.forward;
                    var right = camTransform.right;
                    forward.y = 0;
                    right.y = 0;
                    forward.Normalize();
                    right.Normalize();
                    var moveVelocity = (right * moveInput.x + forward * moveInput.y);
                    actor.MovementController.Move(moveVelocity);
                })
                .RegisterTo(cancellationToken);
        }
    }
}
