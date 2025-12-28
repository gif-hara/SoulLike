using System;
using System.Collections.Generic;
using HK;
using R3;
using R3.Triggers;
using SoulLike.ActorControllers;
using SoulLike.ActorControllers.Abilities;
using UnityEngine;

namespace SoulLike
{
    public sealed class Projectile : MonoBehaviour
    {
        [SerializeField]
        private Collider controlledCollider;

        private readonly HashSet<Actor> hitActors = new();

        public IDisposable Activate(Actor actor, AttackData attackData, int layer)
        {
            controlledCollider.gameObject.layer = layer;
            hitActors.Clear();
            return controlledCollider.OnTriggerStayAsObservable()
                .Subscribe((this, actor, attackData), static (x, t) =>
                {
                    var (@this, actor, attackData) = t;
                    var target = x.attachedRigidbody?.GetComponent<Actor>();
                    if (target == null)
                    {
                        return;
                    }
                    if (@this.hitActors.Contains(target))
                    {
                        return;
                    }
                    var targetStatus = target.GetAbility<ActorStatus>();
                    if (targetStatus == null)
                    {
                        return;
                    }
                    if (targetStatus.IsInvincible.CurrentValue)
                    {
                        return;
                    }
                    if (targetStatus.IsParrying)
                    {
                        target.Event.Broker.Publish(new ActorEvent.OnBeginParry());
                    }
                    else
                    {
                        var takeDamageData = targetStatus.TakeDamage(actor, attackData);
                        var hitPosition = x.ClosestPoint(@this.controlledCollider.transform.position);
                        actor.GetAbility<ActorStatus>().AddSpecialPower(attackData.SpecialPowerRecovery);
                        actor.Event.Broker.Publish(new ActorEvent.OnGiveDamage(attackData, targetStatus.IsStunned, takeDamageData.Damage, hitPosition, target));
                        if (!string.IsNullOrEmpty(attackData.SfxKey))
                        {
                            TinyServiceLocator.Resolve<AudioManager>().PlaySfx(attackData.SfxKey);
                        }
                        if (targetStatus.IsStunned)
                        {
                            if (!string.IsNullOrEmpty(attackData.SfxKeyOnStun))
                            {
                                TinyServiceLocator.Resolve<AudioManager>().PlaySfx(attackData.SfxKeyOnStun);
                            }
                        }
                        foreach (var hitEffectPrefab in attackData.HitEffectPrefabs)
                        {
                            Instantiate(hitEffectPrefab, hitPosition, Quaternion.identity);
                        }
                    }
                    @this.hitActors.Add(target);
                });
        }
    }
}
