using System.Collections.Generic;
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

        private readonly Dictionary<System.Type, IActorAbility> abilities = new();

        public void AddAbility<T>() where T : IActorAbility, new()
        {
            abilities[typeof(T)] = new T();
        }

        public T FindAbility<T>() where T : class, IActorAbility
        {
            abilities.TryGetValue(typeof(T), out var ability);
            return ability as T;
        }

        public Actor Spawn(Vector3 position, Quaternion rotation)
        {
            var actor = Instantiate(this, position, rotation);
            actor.TimeController = new ActorTimeController(actor);
            actor.MovementController = new ActorMovementController();
            actor.AnimationController = new ActorAnimationController(actor);
            actor.BrainController = new ActorBrainController(actor);

            actor.MovementController.Activate(actor);

            return actor;
        }
    }
}
