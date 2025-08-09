using SoulLike.ActorControllers;
using SoulLike.ActorControllers.Abilities;
using SoulLike.ActorControllers.Brains;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SoulLike
{
    public class MainSceneController : MonoBehaviour
    {
        [field: SerializeField]
        private Actor playerPrefab;

        [field: SerializeField]
        private Transform spawnPoint;

        [field: SerializeField]
        private PlayerInput playerInputPrefab;

        [field: SerializeField]
        private Camera worldCamera;

        void Start()
        {
            var player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            var playerInput = Instantiate(playerInputPrefab);
            var brainController = player.AddAbility<ActorBrainController>();
            brainController.Attach(new Player(playerInput, worldCamera));
        }
    }
}
