using UnityEngine;
using System.Collections.Generic;

public class FarmManager : MonoBehaviour {
    public int Gold = 0;
    public int Food = 0;
    public List<bool> enclosuresClean = new List<bool>();

    public void AddGold(int amount) {
        Gold += amount;
    }
    public void AddFood(int amount) {
        Food += amount;
    }
    public void SetEnclosureClean(int index, bool clean) {
        if (index >= 0 && index < enclosuresClean.Count) {
            enclosuresClean[index] = clean;
        }
    }
}

