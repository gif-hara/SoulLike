using R3;
using R3.Triggers;
using UnityEngine;

namespace SoulLike.ActorControllers.Abilities
{
    public sealed class ActorWalk : IActorAbility
    {
        public float MoveSpeed { get; set; }

        public float Acceleration { get; set; } = 10f;

        private ActorAnimation actorAnimation;

        private ActorMovement actorMovement;

        private Vector3 normalizedVelocity;

        private Vector3 currentVelocity;

        public void Activate(Actor actor)
        {
            actorAnimation = actor.GetAbility<ActorAnimation>();
            actorMovement = actor.GetAbility<ActorMovement>();
            var actorStatus = actor.GetAbility<ActorStatus>();
            actor.UpdateAsObservable()
                .Where(actorStatus, static (_, actorStatus) => !actorStatus.IsDead)
                .Subscribe((this, actor), static (_, t) =>
                {
                    var (@this, actor) = t;

                    @this.currentVelocity = Vector3.Lerp(@this.currentVelocity, @this.normalizedVelocity * @this.MoveSpeed, Time.deltaTime * @this.Acceleration);

                    @this.actorMovement.Move(@this.currentVelocity);

                    // 移動量をアニメーションに渡す
                    @this.actorAnimation.SetFloat(ActorAnimation.Parameter.MoveSpeed, @this.currentVelocity.magnitude);

                    var localVelocity = actor.transform.InverseTransformDirection(@this.currentVelocity.normalized);
                    var speed = @this.currentVelocity.sqrMagnitude / (@this.MoveSpeed * @this.MoveSpeed);
                    @this.actorAnimation.SetFloat(ActorAnimation.Parameter.MoveX, localVelocity.x * speed);
                    @this.actorAnimation.SetFloat(ActorAnimation.Parameter.MoveY, localVelocity.z * speed);

                    // 移動入力がある場合、移動方向に向く
                    if (@this.normalizedVelocity.sqrMagnitude > 0.0001f)
                    {
                        var targetRotation = Quaternion.LookRotation(@this.normalizedVelocity, Vector3.up);
                        @this.actorMovement.Rotate(targetRotation);
                    }

                    @this.normalizedVelocity = Vector3.zero;
                })
                .RegisterTo(actor.destroyCancellationToken);
        }

        public void SetNormalizedVelocity(Vector3 normalizedVelocity)
        {
            this.normalizedVelocity = normalizedVelocity;
        }
    }
}
