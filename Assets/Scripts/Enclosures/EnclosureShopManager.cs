using UnityEngine;
using Managers;

namespace Enclosures {
    public class EnclosureShopManager : MonoBehaviour {
        [SerializeField] private EnclosuresManager enclosuresManager;
        [SerializeField] private FarmManager farmManager;

        public int newEnclosurePrice = 100;
        public int upgradePrice = 50;

        public void Start() {
            if (!enclosuresManager) enclosuresManager = FindFirstObjectByType<EnclosuresManager>();
            if (!farmManager) farmManager = FindFirstObjectByType<FarmManager>();
        }

        public void BuyEnclosure(string newName, int capacity) {                // Buy new enclosure
            if (farmManager.gold >= newEnclosurePrice) {
                farmManager.AddGold(-newEnclosurePrice);                        // Remove gold
                enclosuresManager.AddEnclosure(newName, capacity);
                Debug.Log("Nouvel enclos acheté : " + newName);
            } else {
                Debug.Log("Pas assez d'or pour acheter un nouvel enclos.");
            }
        }

        public void UpgradeEnclosure(int index, int extraCapacity) {            // Expand enclosure capacity
            if (index < 0 || index >= enclosuresManager.Enclosures.Count) return;

            if (farmManager.gold >= upgradePrice) {
                farmManager.AddGold(-upgradePrice);
                enclosuresManager.Enclosures[index].IncreaseCapacity(extraCapacity);
                Debug.Log("Enclos " + enclosuresManager.Enclosures[index].NewName + " agrandi de " + extraCapacity + " places.");
            } else {
                Debug.Log("Pas assez d'or pour agrandir l'enclos.");
            }
        }
    }
}