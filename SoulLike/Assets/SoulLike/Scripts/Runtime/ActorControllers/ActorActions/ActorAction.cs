using System.Collections.Generic;
using TNRD;
using UnityEngine;

namespace SoulLike.ActorControllers.ActorActions
{
    [CreateAssetMenu(menuName = "SoulLike/ActorControllers/ActorAction")]
    public class ActorAction : ScriptableObject
    {
        [SerializeField]
        private List<SerializableInterface<IActorAction>> actions = new();

        public void Invoke(Actor actor)
        {
            foreach (var action in actions)
            {
                action.Value.Invoke(actor);
            }
        }
    }
}
