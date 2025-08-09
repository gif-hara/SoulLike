using R3;
using R3.Triggers;
using StandardAssets.Characters.Physics;
using UnityEngine;
using UnityEngine.Assertions;

namespace SoulLike.ActorControllers
{
    public class ActorMovementController : IActorAbility
    {
        private Actor actor;

        private OpenCharacterController openCharacterController;

        private Vector3 velocity;

        private Vector3 velocityFromAnimator;

        public Quaternion TargetRotation { get; private set; }

        private float rotationSpeed = 10.0f;

        private readonly ReactiveProperty<bool> isMoving = new(false);
        public ReadOnlyReactiveProperty<bool> IsMoving => isMoving;

        public readonly ReactiveProperty<bool> CanMove = new(true);

        public readonly ReactiveProperty<bool> CanMoveFromEvent = new(true);

        public readonly ReactiveProperty<bool> CanRotate = new(true);

        public readonly ReactiveProperty<bool> CanRotateFromEvent = new(true);

        public void Move(Vector3 velocity)
        {
            this.velocity = velocity;
        }

        public void MoveFromAnimator(Vector3 velocity)
        {
            velocityFromAnimator = velocity;
        }

        public void Rotate(Quaternion rotation)
        {
            TargetRotation = rotation;
        }

        public void RotateImmediate(Quaternion rotation)
        {
            TargetRotation = rotation;
            actor.transform.rotation = rotation;
        }

        public void SetRotationSpeed(float rotationSpeed)
        {
            this.rotationSpeed = rotationSpeed;
        }

        public void Activate(Actor actor)
        {
            this.actor = actor;
            actor.TryGetComponent(out OpenCharacterController openCharacterController);
            Assert.IsNotNull(openCharacterController);
            this.openCharacterController = openCharacterController;
            actor.UpdateAsObservable()
                .Subscribe(this, static (_, @this) =>
                {
                    var deltaTime = @this.actor.TimeController.Time.deltaTime;
                    if (@this.velocity == Vector3.zero || !@this.CanMove.Value)
                    {
                        @this.isMoving.Value = false;
                    }
                    else
                    {
                        @this.openCharacterController.Move(@this.velocity * deltaTime);
                        @this.isMoving.Value = true;
                    }
                    @this.openCharacterController.Move(@this.velocityFromAnimator);
                    @this.velocity = Vector3.zero;
                    @this.velocityFromAnimator = Vector3.zero;
                    var position = @this.actor.transform.position;
                    position.y = 0.0f;
                    @this.actor.transform.position = position;

                    if (@this.CanRotate.Value)
                    {
                        @this.actor.transform.rotation = Quaternion.Slerp(@this.actor.transform.rotation, @this.TargetRotation, @this.rotationSpeed * deltaTime);
                    }
                })
                .RegisterTo(actor.destroyCancellationToken);
        }
    }
}
