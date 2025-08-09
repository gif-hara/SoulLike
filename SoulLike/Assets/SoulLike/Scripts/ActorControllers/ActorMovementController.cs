using R3;
using R3.Triggers;
using StandardAssets.Characters.Physics;
using UnityEngine;

namespace SoulLike.ActorControllers
{
    public class ActorMovementController
    {
        private Actor actor;

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

        public void Setup(Actor actor, OpenCharacterController openCharacterController)
        {
            this.actor = actor;
            actor.UpdateAsObservable()
                .Subscribe(actor, (_, a) =>
                {
                    var deltaTime = a.TimeController.Time.deltaTime;
                    if (velocity == Vector3.zero || !CanMove.Value)
                    {
                        isMoving.Value = false;
                    }
                    else
                    {
                        openCharacterController.Move(velocity * deltaTime);
                        isMoving.Value = true;
                    }
                    openCharacterController.Move(velocityFromAnimator);
                    velocity = Vector3.zero;
                    velocityFromAnimator = Vector3.zero;
                    var position = a.transform.position;
                    position.y = 0.0f;
                    a.transform.position = position;

                    if (CanRotate.Value)
                    {
                        a.transform.rotation = Quaternion.Slerp(a.transform.rotation, TargetRotation, rotationSpeed * deltaTime);
                    }
                })
                .RegisterTo(actor.destroyCancellationToken);
        }

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
    }
}
