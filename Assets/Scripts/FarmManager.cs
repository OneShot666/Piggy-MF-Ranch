using System.Collections.Generic;
using UnityEngine;
using Managers;
using Pigs;

public class FarmManager : MonoBehaviour {
    public bool hasAutoHarvest;
    public int gold;
    public int food;
    public int energy = 100;                                                    // Daily energy
    public List<bool> enclosuresClean = new();

    public void AddGold(int amount) {
        gold += amount;
    }

    public void AddFood(int amount) {
        food += amount;
    }

    private void UseEnergy(int amount) {
        energy = Mathf.Max(0, energy - amount);
    }

    public void RestoreEnergy() {
        energy = 100;
    }

    public void SetEnclosureClean(int index, bool clean) {
        if (index >= 0 && index < enclosuresClean.Count) {
            enclosuresClean[index] = clean;
        }
    }

    public void SellPig(PigManager pigManager, int pigIndex, int price) {
        if (pigIndex >= 0 && pigIndex < pigManager.Pigs.Count) {
            pigManager.Pigs.RemoveAt(pigIndex);
            AddGold(price);
        }
    }

    public void TrainPig(Pig pig, float speedBoost, float enduranceBoost, int energyCost) {
        if (energy >= energyCost) {
            pig.Speed += speedBoost;
            pig.Rest(enduranceBoost);
            UseEnergy(energyCost);
        }
    }
}