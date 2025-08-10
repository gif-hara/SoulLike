using UnityEngine;

namespace SoulLike.MasterDataSystem
{
    [CreateAssetMenu(fileName = "MasterData", menuName = "SoulLike/MasterData")]
    public class MasterData : ScriptableObject
    {
        [field: SerializeField]
        public PlayerSpec PlayerSpec { get; private set; }
    }
}
