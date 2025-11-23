using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
namespace Items {
    public enum ItemType { Coin, Seed, Food, Potion, Tool, Charm }

    [CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item")]
    public class ItemData : ScriptableObject {                                  // Main class of items
        [Header("Basic Info")]
        public string itemName;
        [TextArea] public string description;

        [Header("Display")]
        public Sprite icon;

        [Header("Data")]
        public ItemType type;
        public int buyPrice;
        public int sellPrice;

        [Header("Stacking")]
        public bool stackable = true;
        public int maxStack = 99;

        [Header("Special Parameters")]
        public int nutritionValue;                                                  // For pig food
        public float growTime;                                                      // For seed
        public ItemData cropProduced;                                               // Collected item after grow
    }
}