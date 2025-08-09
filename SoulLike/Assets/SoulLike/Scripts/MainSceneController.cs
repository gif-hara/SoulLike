using SoulLike.ActorControllers;
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
            playerPrefab.Spawn(spawnPoint.position, spawnPoint.rotation);
        }
    }
}
