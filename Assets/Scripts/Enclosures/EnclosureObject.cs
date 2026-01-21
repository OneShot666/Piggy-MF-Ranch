using System.Collections.Generic;
using UnityEngine;
using Pigs;

namespace Enclosures {
    [System.Serializable]
    public class EnclosureObject {
        [SerializeField] private string name;
        [SerializeField] private int capacity;
        [SerializeField] private float cleanliness = 100f;                      // 0 = dirty, 100 = clean
        [SerializeField] private List<Pig> pigs = new();

        public string NewName => name;

        public EnclosureObject(string newName="Standard Enclosure", int newCapacity = 3) {
            name = newName;
            capacity = newCapacity;
        }

        public bool AddPig(Pig pig) {                                           // Add pig to enclosure if not full
            if (pigs.Count >= capacity) return false;
            pigs.Add(pig);
            return true;
        }

        public void RemovePig(Pig pig) {                                        // Remove pig from enclosure
            if (pigs.Contains(pig)) pigs.Remove(pig);
        }

        public void IncreaseCapacity(int amount) {
            capacity += amount;
        }

        public void DirtyOverTime(float deltaTime, float dirtyRate = 1f) {      // Dirtying up enclosure over time
            cleanliness -= dirtyRate * deltaTime;
            cleanliness = Mathf.Clamp(cleanliness, 0f, 100f);
        }

        public void Clean(float amount) {                                       // Clean enclosure
            cleanliness += amount;
            cleanliness = Mathf.Clamp(cleanliness, 0f, 100f);
        }
    }
}
