using Cysharp.Threading.Tasks;
using HK;
using MH3.ActorControllers;
using R3;
using UnityEngine;

namespace MH3
{
    public class ActorTimeController
    {
        private readonly Actor actor;

        public HK.Time Time { get; } = new HK.Time(HK.Time.Root);

        public Observable<Unit> UpdatedTimeScale => Observable.FromEvent(h => Time.UpdatedTimeScale += h, h => Time.UpdatedTimeScale -= h);

        public ActorTimeController(Actor actor)
        {
            this.actor = actor;
        }

        public UniTask BeginHitStopAsync(float timeScale, float duration)
        {
            return Time.BeginHitStopAsync(duration, timeScale, actor.destroyCancellationToken);
        }
    }
}
