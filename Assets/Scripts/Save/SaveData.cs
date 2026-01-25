using System.Collections.Generic;

namespace Save {
    [System.Serializable]
    public class ItemSaveData {
        public string itemName;                                                 // Save's name to find ScriptableObject
        public int quantity;
    }

    [System.Serializable]
    public class PigSaveData {
        public string pigName;                                                  // Enum convert in integer
        public float health;
        public int color;
        public string rarity;
        public float speed;
        public int generation;
        public int mutationBonus;
        public string activePower;
        public string passivePower;
        public float hunger;
        public int hungerMax;
        public float happiness;
        public int happinessMax;
        public float clean;
        public int cleanMax;
        public float endurance;
        public int enduranceMax;
    }

    [System.Serializable]
    public class TileSaveData {
        public int state;                                                       // Enum convert in integer
        public bool isWet;
        public string seedName;
        public float growTimer;
    }

    [System.Serializable]
    public class FieldSaveData {
        public List<TileSaveData> tiles = new();
    }

    [System.Serializable]
    public class OfferSaveData {
        public string offerName;
        public int quantity;
        public float discount;
    }

    [System.Serializable]
    public class MarketSaveData {
        public int refreshPrice;
        public List<OfferSaveData> offers = new();
    }

    [System.Serializable]
    public class GlobalSaveData {
        public int money;
        public List<ItemSaveData> inventory = new();                            // For items
        public List<PigSaveData> herd = new();                                  // For pigs
        public List<FieldSaveData> cropfield = new();                           // For fields
        public List<MarketSaveData> marketplace = new();                        // For markets
    }
}
