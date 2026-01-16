using UnityEngine;

// ReSharper disable UnassignedField.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
#pragma warning disable CS8524
namespace Items {
    public enum ItemType { Coin, Seed, Food, Potion, Tool, Charm }
    public enum ItemRarity { Common, Uncommon, Rare, Epic, Legendary, Unique }

    [CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item")]
    public class ItemData : ScriptableObject {                                  // Main class of items
        [Header("Basic Info")]
        public string itemName;
        [TextArea] public string description;

        [Header("Display")]
        public Sprite icon;

        [Header("Data")]
        public ItemRarity rarity = ItemRarity.Common;
        public ItemType type;
        public int buyPrice;
        public int sellPrice;

        [Header("Stacking")]
        public bool stackable = true;
        public int maxStack = 99;

        [Header("Special Parameters")]
        public int nutritionValue;                                              // For pig food
        public float growTime;                                                  // For seed
        public ItemData cropProduced;                                           // Collected item after grow
        public float boost;                                                     // For potion
        public float duration;                                                  // If need a timer

        public Color GetRarityColor() {
            return rarity switch {
                ItemRarity.Common => Color.black,
                ItemRarity.Uncommon => Color.green,
                ItemRarity.Rare => Color.blue,
                ItemRarity.Epic => Color.purple,
                ItemRarity.Legendary => Color.gold,
                ItemRarity.Unique => Color.red
            };
        }
    }
}