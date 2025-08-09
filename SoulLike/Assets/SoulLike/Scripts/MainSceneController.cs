using SoulLike.ActorControllers;
using SoulLike.ActorControllers.Brains;
using UnityEngine;

namespace SoulLike
{
    public class MainSceneController : MonoBehaviour
    {
        [field: SerializeField]
        private Actor playerPrefab;

        [field: SerializeField]
        private Transform spawnPoint;

        void Start()
        {
            var player = playerPrefab.Spawn(spawnPoint.position, spawnPoint.rotation);
            player.BrainController.Attach(new Player());
        }
    }
}
