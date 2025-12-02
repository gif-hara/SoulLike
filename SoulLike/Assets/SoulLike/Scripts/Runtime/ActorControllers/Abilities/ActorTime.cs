using Cysharp.Threading.Tasks;
using HK;
using R3;
using UnityEngine;

namespace SoulLike.ActorControllers.Abilities
{
    public sealed class ActorTime : IActorAbility
    {
        private Actor actor;

        public HK.Time Time { get; } = new HK.Time(HK.Time.Root);

        public Observable<Unit> UpdatedTimeScale => Observable.FromEvent(h => Time.UpdatedTimeScale += h, h => Time.UpdatedTimeScale -= h);

        public void Activate(Actor actor)
        {
            this.actor = actor;
        }

        public UniTask BeginHitStopAsync(float timeScale, float duration)
        {
            return Time.BeginHitStopAsync(duration, timeScale, actor.destroyCancellationToken);
        }
    }
}
