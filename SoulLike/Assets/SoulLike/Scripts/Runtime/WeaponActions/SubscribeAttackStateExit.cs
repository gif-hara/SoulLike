using System;
using System.Collections.Generic;
using System.Threading;
using R3;
using SoulLike.ActorControllers;
using SoulLike.ActorControllers.Abilities;
using TNRD;
using UnityEngine;

namespace SoulLike.WeaponActions
{
    [Serializable]
    public sealed class SubscribeAttackStateExit : IWeaponAction
    {
#if UNITY_EDITOR
        [ClassesOnly]
#endif
        [SerializeField]
        private List<SerializableInterface<IWeaponAction>> actions = new();

        public void Invoke(Weapon weapon, Actor actor, CancellationToken scope)
        {
            var actorAnimation = actor.GetAbility<ActorAnimation>();
            actorAnimation.OnStateExitAsObservable(actorAnimation.GetCurrentAttackStateName())
                .Take(1)
                .Subscribe((this, weapon, actor, scope), static (_, t) =>
                {
                    Debug.Log("Attack State Exit Triggered");
                    var (@this, weapon, actor, scope) = t;
                    foreach (var actionInterface in @this.actions)
                    {
                        actionInterface.Value.Invoke(weapon, actor, scope);
                    }
                });
        }
    }
}
