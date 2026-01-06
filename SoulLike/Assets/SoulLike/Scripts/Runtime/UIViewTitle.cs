using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using R3.Triggers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoulLike
{
    public class UIViewTitle : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text versionText;

        [SerializeField]
        private Button startButton;

        [SerializeField]
        private Slider bgmSlider;

        [SerializeField]
        private Slider sfxSlider;

        public async UniTask BeginAsync(UserData userData, AudioManager audioManager, UIViewFade uiViewFade, CancellationToken cancellationToken)
        {
            gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(startButton.gameObject);
            versionText.text = Application.version;
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
                })
                .RegisterTo(cancellationToken);
            Observable.Merge(
                startButton.OnSelectAsObservable(),
                bgmSlider.OnSelectAsObservable(),
                sfxSlider.OnSelectAsObservable()
            )
                .Subscribe(audioManager, static (_, audioManager) =>
                {
                    audioManager.PlaySfx("Select.1");
                })
                .RegisterTo(cancellationToken);
            sfxSlider.OnSelectAsObservable()
                .Subscribe((this, audioManager), static (_, state) =>
                {
                    var (@this, audioManager) = state;
                    Observable.Interval(TimeSpan.FromSeconds(0.5f))
                        .TakeUntil(@this.sfxSlider.OnDeselectAsObservable())
                        .Subscribe(_ =>
                        {
                            audioManager.PlaySfx("Parry.Success.1");
                        });
                })
                .RegisterTo(cancellationToken);
            await startButton.OnClickAsync(cancellationToken);
            audioManager.PlaySfx("Decide.2");
            audioManager.FadeOutBgmAsync(2.0f, 0.0f, cancellationToken).Forget();
            await uiViewFade.BeginAsync(new Color(1.0f, 1.0f, 1.0f, 0.1f), new Color(1.0f, 1.0f, 1.0f, 0.0f), 0.2f, cancellationToken);
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}
