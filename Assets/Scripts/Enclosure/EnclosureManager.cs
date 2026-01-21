using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Enclosure {
    public string Name;
    public float Cleanliness = 100f; // 0 = dirty, 100 = clean
    public int Capacity = 3;
    public List<Pig> Pigs = new List<Pig>();

    public Enclosure(string name, int capacity) {
        Name = name;
        Capacity = capacity;
    }

    // Add a pig if capacity allows
    public bool AddPig(Pig pig) {
        if (Pigs.Count >= Capacity) return false;
        Pigs.Add(pig);
        return true;
    }

    // Remove a pig
    public void RemovePig(Pig pig) {
        if (Pigs.Contains(pig)) Pigs.Remove(pig);
    }

    // Dirting up the enclosure over time
    public void DirtyOverTime(float deltaTime, float dirtyRate = 1f) {
        Cleanliness -= dirtyRate * deltaTime;
        Cleanliness = Mathf.Clamp(Cleanliness, 0f, 100f);
    }

    // Clean the enclosure
    public void Clean(float amount) {
        Cleanliness += amount;
        Cleanliness = Mathf.Clamp(Cleanliness, 0f, 100f);
    }
}

public class EnclosureManager : MonoBehaviour {
    public List<Enclosure> Enclosures = new List<Enclosure>();

    void Start() {
        // Example: create 2 default enclosures
        Enclosures.Add(new Enclosure("Enclos A", 3));
        Enclosures.Add(new Enclosure("Enclos B", 4));
    }

    void Update() {
            // Dirt all the enclosures over time
        foreach (var enc in Enclosures) {
            enc.DirtyOverTime(Time.deltaTime, 0.5f); // 0.5 par seconde, ajustable
        }
    }

    // Add a new pig to an enclosure
    public bool AssignPigToEnclosure(Pig pig, int index) {
        if (index >= 0 && index < Enclosures.Count) {
            return Enclosures[index].AddPig(pig);
        }
        return false;
    }

    // Remove a pig from an enclosure
    public void RemovePigFromEnclosure(Pig pig, int index) {
        if (index >= 0 && index < Enclosures.Count) {
            Enclosures[index].RemovePig(pig);
        }
    }

    // Add a new enclosure
    public void AddEnclosure(string name, int capacity) {
        Enclosures.Add(new Enclosure(name, capacity));
    }
}
