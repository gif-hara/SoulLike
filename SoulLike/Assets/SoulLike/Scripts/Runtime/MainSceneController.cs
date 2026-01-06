using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using R3.Triggers;
using SoulLike.ActorControllers;
using SoulLike.ActorControllers.Abilities;
using SoulLike.ActorControllers.Brains;
using SoulLike.MasterDataSystem;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using unityroom.Api;

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
        private UIViewInputGuide uiViewInputGuide;

        [SerializeField]
        private UIViewTitle uiViewTitle;

        [SerializeField]
        private UIViewEpilogue uiViewEpilogue;

        [SerializeField]
        private UIViewResult uiViewResult;

        [SerializeField]
        private Color fadeInColor;

        [SerializeField]
        private Color fadeOutColor;

        [SerializeField]
        private float fadeDuration = 1f;

        [SerializeField]
        private string battleBgmKey;

        [SerializeField]
        private string enhanceBgmKey;

        [SerializeField]
        private string titleBgmKey;

        [SerializeField]
        private string epilogueBgmKey;

        [SerializeField]
        private string resultBgmKey;

        [SerializeField]
        private Color gameStartBackgroundColor;

        [SerializeField]
        private Color gameStartForwardColor;

        [SerializeField]
        private Color gameStartMessageColor;

        [SerializeField, Multiline]
        private string gameStartMessage;

        [SerializeField]
        private AttackData debugDamageAttackData;

        async UniTaskVoid Start()
        {
            TinyServiceLocator.Register(audioManager)
                .RegisterTo(destroyCancellationToken);
            var sceneBroker = new MessageBroker();
            var worldCameraController = Instantiate(worldCameraControllerPrefab);
            TinyServiceLocator.Register(worldCameraController)
                .RegisterTo(destroyCancellationToken);
            var userData = new UserData();
            var player = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
            var enemy = Instantiate(enemyPrefab, enemySpawnPoint.position, enemySpawnPoint.rotation);

            worldCameraController.BeginObserve(player);
            worldCameraController.SetDefaultCameraTarget(player.transform);
            worldCameraController.SetLockOnCameraTarget(player.transform, enemy.transform);
            worldCameraController.SetActiveLockOnCamera(true);

            mainGlobalVolumeController.BeginObserve(player);

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

            while (!destroyCancellationToken.IsCancellationRequested)
            {
                uiViewEnhance.Initialize();
                uiViewDialog.Initialize();
                uiViewEffectMessage.Initialize();
                uiViewEpilogue.Initialize();
                uiViewResult.SetActive(false);

                audioManager.SetVolumeBgm(userData.bgmVolume.Value);
                audioManager.SetVolumeSfx(userData.sfxVolume.Value);

                audioManager.PlayBgm(titleBgmKey);

                uiViewFade.BeginAsync(fadeOutColor, fadeInColor, fadeDuration, destroyCancellationToken).Forget();
                await uiViewTitle.BeginAsync(userData, audioManager, uiViewFade, destroyCancellationToken);
                await uiViewEffectMessage.BeginAsync(gameStartBackgroundColor, gameStartForwardColor, gameStartMessageColor, gameStartMessage, sceneBroker, destroyCancellationToken, () => uiViewTitle.SetActive(false), () => uiViewFade.BeginAsync(fadeInColor, fadeOutColor, 0.0f, destroyCancellationToken).Forget());

                using var gamePlayScope = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
                this.UpdateAsObservable()
                    .Subscribe(userData, static (_, userData) =>
                    {
                        userData.PlayTime += UnityEngine.Time.deltaTime;
                    })
                    .RegisterTo(gamePlayScope.Token);
                player.Brain.Attach(new Player(playerInput, worldCameraController, masterData.PlayerSpec, userData, sceneBroker, enemy));
                enemy.Brain.Attach(new Enemy(masterData.EnemySpecs[enemySpecId], sceneBroker, player, uiViewEffectMessage));

                uiViewPlayerStatus.Bind(player, userData);
                uiViewEnemyStatus.Bind(enemy);
                uiViewDamageLabel.BeginObserve(player, worldCameraController.WorldCamera);
                uiViewInputGuide.Activate(playerInput, userData, sceneBroker);

                uiViewFade.BeginAsync(fadeOutColor, fadeInColor, fadeDuration, destroyCancellationToken).Forget();

                while (!destroyCancellationToken.IsCancellationRequested)
                {
                    audioManager.PlayBgm(battleBgmKey);
                    var gameJudgement = await sceneBroker.Receive<MainSceneEvent.GameJudgement>()
                        .FirstAsync(destroyCancellationToken)
                        .AsUniTask();
                    if (gameJudgement.Judgement == MainSceneEvent.JudgementType.PlayerWin)
                    {
                        gamePlayScope.Cancel();
                        gamePlayScope.Dispose();
                        await uiViewFade.BeginAsync(fadeInColor, fadeOutColor, 3.0f, destroyCancellationToken);
                        await UniTask.Delay(TimeSpan.FromSeconds(2.0f), cancellationToken: destroyCancellationToken);
                        audioManager.PlayBgm(epilogueBgmKey);
                        uiViewFade.BeginAsync(fadeOutColor, fadeInColor, 0.25f, destroyCancellationToken).Forget();
                        var epilogueResult = await uiViewEpilogue.BeginAsync(playerInput, destroyCancellationToken);
                        if (epilogueResult == UIViewEpilogue.ResultType.Skip)
                        {
                            audioManager.FadeOutBgmAsync(0.5f, 0.0f, destroyCancellationToken).Forget();
                            await uiViewFade.BeginAsync(fadeInColor, fadeOutColor, 1.0f, destroyCancellationToken);
                        }
                        else
                        {
                            await audioManager.FadeOutBgmAsync(1.0f, 0.0f, destroyCancellationToken);
                            await uiViewFade.BeginAsync(fadeInColor, fadeOutColor, 1.0f, destroyCancellationToken);
                        }
                        audioManager.PlayBgm(resultBgmKey);
                        UnityroomApiClient.Instance.SendScore(1, userData.PlayTime, ScoreboardWriteMode.HighScoreAsc);
                        UnityroomApiClient.Instance.SendScore(2, userData.DeadCount, ScoreboardWriteMode.HighScoreAsc);
                        await uiViewResult.BeginAsync(userData, audioManager, uiViewFade, destroyCancellationToken);
                        break;
                    }
                    else if (gameJudgement.Judgement == MainSceneEvent.JudgementType.PlayerLose)
                    {
                        await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: destroyCancellationToken);
                        await uiViewFade.BeginAsync(fadeInColor, fadeOutColor, fadeDuration, destroyCancellationToken);
                        audioManager.PlayBgm(enhanceBgmKey);
                        await uiViewEnhance.BeginAsync(masterData, userData, playerInput, uiViewFade, uiViewDialog, userData.DeadCount == 1, destroyCancellationToken);
                        sceneBroker.Publish(new MainSceneEvent.RestartGame(player, enemy));
                        await uiViewFade.BeginAsync(fadeOutColor, fadeInColor, fadeDuration, destroyCancellationToken);
                    }
                }

                player.Brain.Attach(null);
                enemy.Brain.Attach(null);
                player.transform.position = playerSpawnPoint.position;
                player.transform.rotation = playerSpawnPoint.rotation;
                enemy.transform.position = enemySpawnPoint.position;
                enemy.transform.rotation = enemySpawnPoint.rotation;
                userData.Reset();
            }
        }
    }
}
