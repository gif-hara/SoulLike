using HK;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SoulLike
{
    public class UIViewInputGuide : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text inputGuideText;

        [SerializeField]
        private string[] replaceActionNames;

        private string format;

        public void Activate(PlayerInput playerInput)
        {
            format = inputGuideText.text;
            playerInput.OnControlsChangedAsObservable()
                .Prepend(playerInput)
                .Subscribe((this, playerInput), static (_, t) =>
                {
                    var (@this, playerInput) = t;
                    var message = @this.format;
                    foreach (var actionName in @this.replaceActionNames)
                    {
                        message = message.Replace("{" + actionName + "}", InputSprite.GetTag(playerInput, playerInput.actions[actionName]));
                    }
                    @this.inputGuideText.SetText(message);
                })
                .RegisterTo(destroyCancellationToken);
        }
    }
}
