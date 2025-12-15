using System;
using System.Collections.Generic;
using System.Threading;
using R3;
using SoulLike.ActorControllers;
using TNRD;
using UnityEngine;

namespace SoulLike.WeaponActions
{
    [Serializable]
    public sealed class SubscribeOnRecoveryHitPoint : IWeaponAction
    {
#if UNITY_EDITOR
        [ClassesOnly]
#endif
        [SerializeField]
        private List<SerializableInterface<IWeaponAction>> actions = new();

        public void Invoke(Weapon weapon, Actor actor, CancellationToken scope)
        {
            actor.Event.Broker.Receive<ActorEvent.OnRecoveryHitPoint>()
                .Subscribe((this, weapon, actor, scope), static (_, t) =>
                {
                    var (@this, weapon, actor, scope) = t;
                    foreach (var actionInterface in @this.actions)
                    {
                        actionInterface.Value.Invoke(weapon, actor, scope);
                    }
                })
                .RegisterTo(scope);
        }
    }
}
