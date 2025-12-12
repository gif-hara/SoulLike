using System;
using HK;
using UnityEngine;

namespace SoulLike.ActorControllers.ActorActions
{
    [Serializable]
    public sealed class PlaySfx : IActorAction
    {
        [SerializeField]
        private string sfxKey;

        public void Invoke(Actor actor)
        {
            TinyServiceLocator.Resolve<AudioManager>().PlaySfx(sfxKey);
        }
    }
}
