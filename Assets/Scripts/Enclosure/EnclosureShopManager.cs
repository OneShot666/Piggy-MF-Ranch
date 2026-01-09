using UnityEngine;

public class EnclosureShopManager : MonoBehaviour {
    public EnclosureManager enclosureManager;
    public FarmManager farmManager;

    public int newEnclosurePrice = 100;
    public int upgradePrice = 50;

    // Buy a new enclosure
    public void BuyEnclosure(string name, int capacity) {
        if (farmManager.Gold >= newEnclosurePrice) {
            farmManager.AddGold(-newEnclosurePrice); // remove gold
            enclosureManager.AddEnclosure(name, capacity);
            Debug.Log("Nouvel enclos acheté : " + name);
        } else {
            Debug.Log("Pas assez d'or pour acheter un nouvel enclos.");
        }
    }

    // Expand an enclosure
    public void UpgradeEnclosure(int index, int extraCapacity) {
        if (index < 0 || index >= enclosureManager.Enclosures.Count) return;

        if (farmManager.Gold >= upgradePrice) {
            farmManager.AddGold(-upgradePrice);
            enclosureManager.Enclosures[index].Capacity += extraCapacity;
            Debug.Log("Enclos " + enclosureManager.Enclosures[index].Name + " agrandi de " + extraCapacity + " places.");
        } else {
            Debug.Log("Pas assez d'or pour agrandir l'enclos.");
        }
    }
}