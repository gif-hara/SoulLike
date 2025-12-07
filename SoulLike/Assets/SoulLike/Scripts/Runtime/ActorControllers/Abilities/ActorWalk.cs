using R3;
using R3.Triggers;
using UnityEngine;

namespace SoulLike.ActorControllers.Abilities
{
    public sealed class ActorWalk : IActorAbility
    {
        public float MoveSpeed { get; set; }

        private ActorAnimation actorAnimation;

        private ActorMovement actorMovement;

        private Vector3 normalizedVelocity;

        public void Activate(Actor actor)
        {
            actorAnimation = actor.GetAbility<ActorAnimation>();
            actorMovement = actor.GetAbility<ActorMovement>();
            actor.UpdateAsObservable()
                .Subscribe((this, actor), static (_, t) =>
                {
                    var (@this, actor) = t;

                    @this.actorMovement.Move(@this.normalizedVelocity * @this.MoveSpeed);

                    // 移動量をアニメーションに渡す
                    @this.actorAnimation.SetFloat(ActorAnimation.Parameter.MoveSpeed, @this.normalizedVelocity.magnitude);

                    var localVelocity = actor.transform.InverseTransformDirection(@this.normalizedVelocity);
                    @this.actorAnimation.SetFloat(ActorAnimation.Parameter.MoveX, localVelocity.x);
                    @this.actorAnimation.SetFloat(ActorAnimation.Parameter.MoveY, localVelocity.z);

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
