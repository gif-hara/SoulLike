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

        public Player(PlayerInput playerInput)
        {
            this.playerInput = playerInput;
        }

        public void Attach(Actor actor, CancellationToken cancellationToken)
        {
            actor.UpdateAsObservable()
                .Subscribe((this, actor), static (_, t) =>
                {
                    var (@this, actor) = t;
                    var moveVelocity = @this.playerInput.actions["Move"].ReadValue<Vector2>();
                    Debug.Log($"Player Move: {moveVelocity}");
                })
                .RegisterTo(cancellationToken);
        }
    }
}
