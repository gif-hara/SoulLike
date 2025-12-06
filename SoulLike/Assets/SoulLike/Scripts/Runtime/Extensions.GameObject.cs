using UnityEngine;

namespace HK
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class Extensions
    {
        public static void SetLayerRecursive(this GameObject self, int layer)
        {
            self.layer = layer;
            foreach (Transform child in self.transform)
            {
                child.gameObject.SetLayerRecursive(layer);
            }
        }
    }
}