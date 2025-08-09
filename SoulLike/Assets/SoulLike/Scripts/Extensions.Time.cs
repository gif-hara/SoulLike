using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SoulLike
{
    public static partial class Extensions
    {
        private static readonly Dictionary<HK.Time, Stack<float>> timeScaleStacks = new();

        public static async UniTask BeginHitStopAsync(this HK.Time time, float duration, float timeScale, CancellationToken scope)
        {
            if (duration <= 0)
            {
                return;
            }

            if (!timeScaleStacks.TryGetValue(time, out var stack))
            {
                stack = new Stack<float>();
                stack.Push(1.0f);
                timeScaleStacks.Add(time, stack);
            }

            try
            {
                stack.Push(timeScale);
                time.timeScale = timeScale;
                await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: scope);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                stack.Pop();
                time.timeScale = stack.Peek();
            }
        }

        public static void PushTimeScale(this HK.Time time, float timeScale)
        {
            if (!timeScaleStacks.TryGetValue(time, out var stack))
            {
                stack = new Stack<float>();
                stack.Push(1.0f);
                timeScaleStacks.Add(time, stack);
            }

            stack.Push(timeScale);
            time.timeScale = timeScale;
        }

        public static void PopTimeScale(this HK.Time time)
        {
            if (!timeScaleStacks.TryGetValue(time, out var stack))
            {
                return;
            }

            stack.Pop();
            time.timeScale = stack.Peek();
        }
    }
}
