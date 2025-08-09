using StandardAssets.Characters.Physics;
using UnityEngine;

namespace SoulLike.ActorControllers
{
    public class Actor : MonoBehaviour
    {
        [SerializeField]
        private LocatorHolder locatorHolder;

        [SerializeField]
        private OpenCharacterController openCharacterController;

        public ActorMovementController MovementController { get; private set; }

        public ActorAnimationController AnimationController { get; private set; }

        public ActorTimeController TimeController { get; private set; }

        public ActorBrainController BrainController { get; private set; }

        public Actor Spawn(Vector3 position, Quaternion rotation)
        {
            var actor = Instantiate(this, position, rotation);
            actor.TimeController = new ActorTimeController(actor);
            actor.MovementController = new ActorMovementController();
            actor.AnimationController = new ActorAnimationController(actor);
            actor.BrainController = new ActorBrainController(actor);

            actor.MovementController.Setup(actor, actor.openCharacterController);

            return actor;
        }
    }
}
