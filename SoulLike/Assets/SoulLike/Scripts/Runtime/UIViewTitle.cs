using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoulLike
{
    public class UIViewTitle : MonoBehaviour
    {
        [SerializeField]
        private Button startButton;

        [SerializeField]
        private Slider bgmSlider;

        [SerializeField]
        private Slider sfxSlider;

        public async UniTask BeginAsync(UserData userData, AudioManager audioManager, CancellationToken cancellationToken)
        {
            gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(startButton.gameObject);
            bgmSlider.value = userData.bgmVolume.Value;
            sfxSlider.value = userData.sfxVolume.Value;
            bgmSlider.OnValueChangedAsObservable()
                .Skip(1)
                .Subscribe((userData, audioManager), static (x, state) =>
                {
                    var (userData, audioManager) = state;
                    userData.bgmVolume.Value = x;
                    audioManager.SetVolumeBgm(x);
                })
                .RegisterTo(cancellationToken);
            sfxSlider.OnValueChangedAsObservable()
                .Skip(1)
                .Subscribe((userData, audioManager), static (x, state) =>
                {
                    var (userData, audioManager) = state;
                    userData.sfxVolume.Value = x;
                    audioManager.SetVolumeSfx(x);
                    audioManager.PlaySfx("Parry.Success.1");
                })
                .RegisterTo(cancellationToken);
            await startButton.OnClickAsync(cancellationToken);
            audioManager.PlaySfx("Decide.2");
            audioManager.FadeOutBgmAsync(2.0f, 0.0f, cancellationToken).Forget();
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}
