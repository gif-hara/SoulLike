using UnityEngine;

namespace MH3.ActorControllers
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
            actor.MovementController.MoveFromAnimator(animator.deltaPosition);
        }
    }
}
