using HK;
using R3;
using SoulLike.ActorControllers;
using SoulLike.ActorControllers.Abilities;
using SoulLike.ActorControllers.Brains;
using SoulLike.MasterDataSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SoulLike
{
    public class MainSceneController : MonoBehaviour
    {
        [field: SerializeField]
        private MasterData masterData;

        [field: SerializeField]
        private int enemySpecId = 0;

        [field: SerializeField]
        private Actor playerPrefab;

        [field: SerializeField]
        private Actor enemyPrefab;

        [field: SerializeField]
        private Transform playerSpawnPoint;

        [field: SerializeField]
        private Transform enemySpawnPoint;

        [field: SerializeField]
        private PlayerInput playerInputPrefab;

        [field: SerializeField]
        private WorldCameraController worldCameraControllerPrefab;

        [SerializeField]
        private MainGlobalVolumeController mainGlobalVolumeController;

        [SerializeField]
        private AudioManager audioManager;

        [SerializeField]
        private UIViewPlayerStatus uiViewPlayerStatus;

        [SerializeField]
        private UIViewEnemyStatus uiViewEnemyStatus;

        void Start()
        {
            TinyServiceLocator.Register(audioManager)
                .RegisterTo(destroyCancellationToken);
            var worldCameraController = Instantiate(worldCameraControllerPrefab);
            var userData = new UserData();
            var player = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
            var playerInput = Instantiate(playerInputPrefab);
            player.Brain.Attach(new Player(playerInput, worldCameraController.WorldCamera, masterData.PlayerSpec, userData));

            var enemy = Instantiate(enemyPrefab, enemySpawnPoint.position, enemySpawnPoint.rotation);
            enemy.Brain.Attach(new Enemy(masterData.EnemySpecs[enemySpecId]));

            player.GetAbility<ActorTargetHandler>().BeginLockOn(enemy);
            enemy.GetAbility<ActorTargetHandler>().BeginLockOn(player);

            worldCameraController.SetDefaultCameraTarget(player.transform);
            worldCameraController.SetLockOnCameraTarget(player.transform, enemy.transform);
            worldCameraController.SetActiveLockOnCamera(true);

            mainGlobalVolumeController.BeginObserve(player);

            uiViewPlayerStatus.Bind(player, userData);
            uiViewEnemyStatus.Bind(enemy);
        }
    }
}
