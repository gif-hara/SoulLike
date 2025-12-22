using System;
using System.Collections.Generic;
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
        private TMP_Text promptText;

        [SerializeField]
        private TMP_Text guideText;

        [SerializeField]
        private string[] replaceActionNames;

        [SerializeField]
        private string attackTag;

        [SerializeField]
        private string attackGuideText;

        [SerializeField]
        private string dodgeTag;

        [SerializeField]
        private List<ActionElement> dodgeActionElements;

        [SerializeField]
        private string parryTag;

        [SerializeField]
        private List<ActionElement> parryActionElements;

        [SerializeField]
        private string strongAttackTag;

        [SerializeField]
        private List<ActionElement> strongAttackActionElements;

        private string promptFormat;

        private string guideFormat;

        public void Activate(PlayerInput playerInput, UserData userData, IMessageReceiver sceneBroker)
        {
            promptFormat = promptText.text;
            guideFormat = guideText.text;
            Observable.Merge(
                playerInput.OnControlsChangedAsObservable().Prepend(playerInput).AsUnitObservable(),
                sceneBroker.Receive<MainSceneEvent.RestartGame>().AsUnitObservable()
            )
                .Subscribe((this, playerInput, userData), static (_, t) =>
                {
                    var (@this, playerInput, userData) = t;
                    var promptMessage = @this.promptFormat;
                    foreach (var actionName in @this.replaceActionNames)
                    {
                        promptMessage = promptMessage.Replace("{" + actionName + "}", InputSprite.GetTag(playerInput, playerInput.actions[actionName]));
                    }
                    @this.promptText.SetText(promptMessage);

                    var guideMessage = @this.guideFormat;
                    guideMessage = guideMessage
                        .Replace(@this.attackTag, @this.attackGuideText)
                        .Replace(@this.dodgeTag, @this.dodgeActionElements.Find(x => x.id == userData.DodgeUniqueAttackId).actionName)
                        .Replace(@this.parryTag, @this.parryActionElements.Find(x => x.id == userData.ParryUniqueAttackId).actionName)
                        .Replace(@this.strongAttackTag, @this.strongAttackActionElements.Find(x => x.id == userData.StrongAttackUniqueAttackId).actionName);
                    @this.guideText.SetText(guideMessage);
                })
                .RegisterTo(destroyCancellationToken);
        }

        [Serializable]
        public struct ActionElement
        {
            public int id;

            public string actionName;
        }
    }
}
