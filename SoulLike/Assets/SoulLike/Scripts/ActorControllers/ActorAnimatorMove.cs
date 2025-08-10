using SoulLike.ActorControllers.Abilities;
using UnityEngine;

namespace SoulLike.ActorControllers
{
    public class ActorAnimatorMove : MonoBehaviour
    {
        [field: SerializeField]
        private Animator animator;

        private Actor actor;

        public void Activate(Actor actor)
        {
            this.actor = actor;
        }

        void OnAnimatorMove()
        {
            if (actor == null || animator == null)
            {
                return;
            }
            actor.GetAbility<ActorMovement>().MoveFromAnimator(animator.deltaPosition);
        }
    }
}
