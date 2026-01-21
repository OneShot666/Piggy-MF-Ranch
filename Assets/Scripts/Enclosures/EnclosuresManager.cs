using System.Collections.Generic;
using UnityEngine;
using Pigs;

namespace Enclosures {
    public class EnclosuresManager : MonoBehaviour {
        [SerializeField] private List<EnclosureObject> enclosures = new();
        [SerializeField] private float dirtySpeed = 0.5f;
        
        public List<EnclosureObject> Enclosures => enclosures;

        void Start() {
            enclosures.Add(new EnclosureObject("Enclos A", 2));   // !! Examples of enclosures
            enclosures.Add(new EnclosureObject("Enclos B"));
            enclosures.Add(new EnclosureObject("Enclos C", 4));
        }

        void Update() {                                                         // Dirt all enclosures over time
            foreach (var enc in enclosures) enc.DirtyOverTime(Time.deltaTime, dirtySpeed);
        }

        public bool AssignPigToEnclosure(Pig pig, int index) {                  // Move pig to given enclosure
            return index >= 0 && index < enclosures.Count && enclosures[index].AddPig(pig);
        }

        public void RemovePigFromEnclosure(Pig pig, int index) {                // Remove pig from given enclosure
            if (index >= 0 && index < enclosures.Count)
                enclosures[index].RemovePig(pig);
        }

        public void AddEnclosure(string enclosureName, int capacity) {          // Add new enclosure to list
            enclosures.Add(new EnclosureObject(enclosureName, capacity));
        }
    }
}