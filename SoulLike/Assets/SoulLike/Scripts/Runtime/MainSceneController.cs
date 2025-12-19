using System;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using R3.Triggers;
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
        private PlayerInput playerInput;

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

        [SerializeField]
        private UIViewFade uiViewFade;

        [SerializeField]
        private UIViewEnhance uiViewEnhance;

        [SerializeField]
        private UIViewDialog uiViewDialog;

        [SerializeField]
        private UIViewDamageLabel uiViewDamageLabel;

        [SerializeField]
        private UIViewEffectMessage uiViewEffectMessage;

        [SerializeField]
        private Color fadeInColor;

        [SerializeField]
        private Color fadeOutColor;

        [SerializeField]
        private float fadeDuration = 1f;

        [SerializeField]
        private AttackData debugDamageAttackData;

        async UniTaskVoid Start()
        {
            TinyServiceLocator.Register(audioManager)
                .RegisterTo(destroyCancellationToken);
            var sceneBroker = new MessageBroker();
            var worldCameraController = Instantiate(worldCameraControllerPrefab);
            var userData = new UserData();
            var player = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
            player.Brain.Attach(new Player(playerInput, worldCameraController, masterData.PlayerSpec, userData, sceneBroker));

            var enemy = Instantiate(enemyPrefab, enemySpawnPoint.position, enemySpawnPoint.rotation);
            enemy.Brain.Attach(new Enemy(masterData.EnemySpecs[enemySpecId], sceneBroker, player));

            player.GetAbility<ActorTargetHandler>().BeginLockOn(enemy);
            enemy.GetAbility<ActorTargetHandler>().BeginLockOn(player);

            worldCameraController.BeginObserve(player);
            worldCameraController.SetDefaultCameraTarget(player.transform);
            worldCameraController.SetLockOnCameraTarget(player.transform, enemy.transform);
            worldCameraController.SetActiveLockOnCamera(true);

            mainGlobalVolumeController.BeginObserve(player);

            uiViewEnhance.Initialize();
            uiViewPlayerStatus.Bind(player, userData);
            uiViewEnemyStatus.Bind(enemy);
            uiViewDialog.Initialize();
            uiViewDamageLabel.BeginObserve(player, worldCameraController.WorldCamera);
            uiViewEffectMessage.Initialize();

#if DEBUG
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    if (Keyboard.current.f1Key.wasPressedThisFrame)
                    {
                        player.GetAbility<ActorStatus>().TakeDamage(player, debugDamageAttackData);
                    }
                    if (Keyboard.current.f2Key.wasPressedThisFrame)
                    {
                        enemy.GetAbility<ActorStatus>().TakeDamage(enemy, debugDamageAttackData);
                    }
                    if (Keyboard.current.f3Key.wasPressedThisFrame)
                    {
                        player.GetAbility<ActorStatus>().AddSpecialPower(1.0f);
                    }
                    if (Keyboard.current.f4Key.wasPressedThisFrame)
                    {
                        userData.AddExperience(100000);
                    }
                })
                .RegisterTo(destroyCancellationToken);
#endif

            uiViewFade.BeginAsync(fadeOutColor, fadeInColor, fadeDuration, destroyCancellationToken).Forget();

            while (!destroyCancellationToken.IsCancellationRequested)
            {
                var gameJudgement = await sceneBroker.Receive<MainSceneEvent.GameJudgement>()
                    .FirstAsync(destroyCancellationToken)
                    .AsUniTask();
                if (gameJudgement.Judgement == MainSceneEvent.JudgementType.PlayerWin)
                {
                    Debug.Log("Player Win!");
                }
                else if (gameJudgement.Judgement == MainSceneEvent.JudgementType.PlayerLose)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: destroyCancellationToken);
                    await uiViewFade.BeginAsync(fadeInColor, fadeOutColor, fadeDuration, destroyCancellationToken);
                    await uiViewEnhance.BeginAsync(masterData, userData, playerInput, uiViewFade, uiViewDialog, userData.DeadCount == 1, destroyCancellationToken);
                    sceneBroker.Publish(new MainSceneEvent.RestartGame(player, enemy));
                    await uiViewFade.BeginAsync(fadeOutColor, fadeInColor, fadeDuration, destroyCancellationToken);
                }
            }
        }
    }
}
