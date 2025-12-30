using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoulLike
{
    public class UIViewResult : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text playTimeText;

        [SerializeField]
        private TMP_Text deadCountText;

        [SerializeField]
        private TMP_Text totalExperienceText;

        [SerializeField]
        private Button toTitleButton;

        public async UniTask BeginAsync(UserData userData, AudioManager audioManager, UIViewFade uiViewFade, CancellationToken cancellationToken)
        {
            gameObject.SetActive(true);

            playTimeText.SetText(TimeSpan.FromSeconds(userData.PlayTime).ToString(@"mm\:ss"));
            deadCountText.SetText(userData.DeadCount.ToString());
            totalExperienceText.SetText(userData.TotalExperience.ToString());
            EventSystem.current.SetSelectedGameObject(toTitleButton.gameObject);

            await uiViewFade.BeginAsync(0.0f, 1.0f, cancellationToken);
            await toTitleButton.OnClickAsync(cancellationToken);
            audioManager.PlaySfx("Decide.2");
            await uiViewFade.BeginAsync(1.0f, 1.0f, cancellationToken);
            gameObject.SetActive(false);
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}
