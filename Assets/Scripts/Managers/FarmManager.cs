using System.Collections.Generic;
using Basic;
using Managers;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
public class FarmManager : MonoBehaviour {
    public int gold;
    public int food;
    public int energy = 100; // Daily energy
    public List<bool> enclosuresClean = new();

    public void AddGold(int amount) {
        gold += amount;
    }

    public void AddFood(int amount) {
        food += amount;
    }

    public void UseEnergy(int amount) {
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
        if (pigIndex < 0 || pigIndex >= pigManager.pigs.Count) return;

        pigManager.pigs.RemoveAt(pigIndex);
        AddGold(price);
    }

    public void TrainPig(Pig pig, float speedBoost, float enduranceBoost, int energyCost) {
        if (energy < energyCost) return;

        pig.Speed += speedBoost;
        pig.Endurance += enduranceBoost;
        UseEnergy(energyCost);
    }
}
