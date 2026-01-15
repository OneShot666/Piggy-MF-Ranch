using UnityEngine;
using Items;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
namespace Market {
    [CreateAssetMenu(fileName = "MarketItemPool", menuName = "Game/Market Item Pool")]
    public class MarketItemPool : ScriptableObject {
        public ItemData[] possibleItems;
    }
}
