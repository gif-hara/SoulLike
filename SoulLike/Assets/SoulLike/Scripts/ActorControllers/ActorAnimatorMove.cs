using SoulLike.ActorControllers.Abilities;
using UnityEngine;

namespace SoulLike.ActorControllers
{
    public class ActorAnimatorMove : MonoBehaviour
    {
        [SerializeField]
        private Actor actor;

        [SerializeField]
        private Animator animator;

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
