using System.Collections.Generic;
using SoulLike.ShopActions;
using TNRD;
using UnityEngine;

namespace SoulLike
{
    [CreateAssetMenu(menuName = "SoulLike/ShopElement")]
    public class ShopElement : ScriptableObject
    {
        [field: SerializeField]
        public string ElementName { get; private set; }

        [field: SerializeField]
        public string Description { get; private set; }

        [field: SerializeField]
        public int[] Prices { get; private set; }

        [field: SerializeField]
        public bool CanPurchaseInfinitely { get; private set; }

#if UNITY_EDITOR
        [ClassesOnly]
#endif
        [field: SerializeField]
        public List<SerializableInterface<IShopAction>> Actions { get; private set; } = new();
    }
}
