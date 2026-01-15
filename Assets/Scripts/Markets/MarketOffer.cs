using UnityEngine;
using Items;

// ReSharper disable UnusedMember.Global
namespace Market {
    [System.Serializable]
    public class MarketOffer {                                                  // Used in markets
        public ItemData item;
        public int quantity;
        public int basePrice;
        public float discount;                                                  // ex : 0.2f = 20%

        public int FinalPrice => Mathf.RoundToInt(basePrice * (1f - discount));

        public string GetDiscountText() => $"-{Mathf.RoundToInt(discount * 100)}%";

        public Color GetDiscountColor() {
            int percent = Mathf.RoundToInt(discount * 100);

            if (percent < 25) return Color.white;
            if (percent < 50) return Color.green;
            if (percent < 75) return Color.cyan;
            return Color.gold;
        }
    }
}
