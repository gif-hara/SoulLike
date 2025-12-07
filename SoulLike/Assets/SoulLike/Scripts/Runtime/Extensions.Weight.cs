using System;
using System.Collections.Generic;

namespace HK
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class Extensions
    {
        public static T Lottery<T>(this IList<T> self, Func<T, int> weightSelector)
        {
            int totalWeight = 0;
            for (int i = 0; i < self.Count; i++)
            {
                totalWeight += weightSelector(self[i]);
            }

            int randomValue = UnityEngine.Random.Range(0, totalWeight);
            int cumulativeWeight = 0;

            for (int i = 0; i < self.Count; i++)
            {
                cumulativeWeight += weightSelector(self[i]);
                if (randomValue < cumulativeWeight)
                {
                    return self[i];
                }
            }

            throw new InvalidOperationException("Lottery failed to select an item.");
        }
    }
}