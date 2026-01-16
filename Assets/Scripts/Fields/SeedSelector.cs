using System.Collections.Generic;
using UnityEngine;
using Items;

namespace Fields {
    public class SeedSelector : MonoBehaviour {
        [SerializeField] private GameObject root;
        [SerializeField] private Transform container;
        [SerializeField] private GameObject seedButtonPrefab;

        private FieldPlot _currentPlot;                                         // Selected field

        public static SeedSelector Instance { get; private set; }

        void Awake() {
            Instance = this;
            root.SetActive(false);                                              // Hide by default
        }

        public void Open(FieldPlot plot) {
            root.SetActive(true);
            _currentPlot = plot;
            RefreshUI();
        }

        private void Close() {
            root.SetActive(false);
            _currentPlot = null;
        }

        private void RefreshUI() {
            foreach (Transform child in container) Destroy(child.gameObject);

            if (!InventoryManager.Instance) return;

            Dictionary<ItemData, int> seedCounts = new Dictionary<ItemData, int>(); // Get seed list from inventory
            foreach(var item in InventoryManager.Instance.items)
                if (item.data.type == ItemType.Seed) {
                    if (seedCounts.ContainsKey(item.data)) seedCounts[item.data] += item.quantity;
                    else seedCounts.Add(item.data, item.quantity);
                }

            foreach(var pair in seedCounts) {
                GameObject btn = Instantiate(seedButtonPrefab, container);
                SeedButton script = btn.GetComponent<SeedButton>();

                if (script) script.SetSeed(pair.Key, pair.Value, () => { OnSelectSeed(pair.Key); });
            }
        }

        private void OnSelectSeed(ItemData seed) {
            InventoryManager.Instance.SetSelectedSeed(seed);
            if(_currentPlot) _currentPlot.PlantMax(seed);
            Close();
        }
    }
}
