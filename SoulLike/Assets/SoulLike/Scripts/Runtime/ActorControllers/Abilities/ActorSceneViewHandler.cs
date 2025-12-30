using System.Threading;
using UnityEngine.Assertions;

namespace SoulLike.ActorControllers.Abilities
{
    public class ActorSceneViewHandler : IActorAbility
    {
        public SceneView SceneView { get; private set; }

        public void Activate(Actor actor, CancellationToken cancellationToken)
        {
            SceneView = actor.GetComponentInChildren<SceneView>();
            Assert.IsNotNull(SceneView, $"SceneView component not found on actor {actor.name} or its children.");
        }
    }
}
