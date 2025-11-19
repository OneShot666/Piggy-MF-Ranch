using UnityEngine;
using Items;

// ReSharper disable UnusedMember.Global
namespace Market {
    [System.Serializable]
    public class MarketOffer {
        public ItemData item;
        public int quantity;
        public int basePrice;
        public float discount; // ex : 0.2f = 20%

        public int FinalPrice => Mathf.RoundToInt(basePrice * (1f - discount));
    }
}
