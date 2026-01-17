using System.Collections.Generic;

namespace Save {
    [System.Serializable]
    public class ItemSaveData {
        public string itemName;                                                 // Save's name to find ScriptableObject
        public int quantity;
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
    public class GlobalSaveData {
        public int money;
        public List<ItemSaveData> inventory = new();
        public List<FieldSaveData> fields = new();
    }
}
