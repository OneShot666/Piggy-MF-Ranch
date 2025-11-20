using UnityEngine;
using System.Collections.Generic;

public class FarmManager : MonoBehaviour {
    public int Gold = 0;
    public int Food = 0;
    public int Energy = 100; // Daily energy
    public List<bool> enclosuresClean = new List<bool>();

    public void AddGold(int amount) {
        Gold += amount;
    }
    public void AddFood(int amount) {
        Food += amount;
    }
    public void UseEnergy(int amount) {
        Energy = Mathf.Max(0, Energy - amount);
    }
    public void RestoreEnergy() {
        Energy = 100;
    }
    public void SetEnclosureClean(int index, bool clean) {
        if (index >= 0 && index < enclosuresClean.Count) {
            enclosuresClean[index] = clean;
        }
    }
    public void SellPig(PigManager pigManager, int pigIndex, int price) {
        if (pigIndex >= 0 && pigIndex < pigManager.pigs.Count) {
            pigManager.pigs.RemoveAt(pigIndex);
            AddGold(price);
        }
    }
    public void TrainPig(Pig pig, float speedBoost, float enduranceBoost, int energyCost) {
        if (Energy >= energyCost) {
            pig.Speed += speedBoost;
            pig.Endurance += enduranceBoost;
            UseEnergy(energyCost);
        }
    }
}
