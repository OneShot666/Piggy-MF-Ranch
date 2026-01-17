using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Items;

namespace Fields {
    public class SeedSelector : MonoBehaviour {
        [Header("References")]
        [SerializeField] private GameObject root;
        [SerializeField] private Transform container;
        [SerializeField] private GameObject seedButtonPrefab;

        private FieldPlot _currentPlot;                                         // Selected field

        public static SeedSelector Instance { get; private set; }

        void Awake() {
            Instance = this;
            root.SetActive(false);                                              // Hide by default
        }

        private void Update() {
            if (root.activeSelf && Keyboard.current.escapeKey.wasPressedThisFrame) Close();
        }

        public void Open(FieldPlot plot) {
            if (!HasAnySeed()) return;                                          // Has to have at least one seed to open

            root.SetActive(true);
            _currentPlot = plot;
            RefreshUI();
        }

        public void Close() {
            root.SetActive(false);
            _currentPlot = null;
        }

        private bool HasAnySeed() {
            if (!InventoryManager.Instance) return false;

            foreach (var item in InventoryManager.Instance.items) if (item.data.type == ItemType.Seed) return true;

            return false;
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

            Canvas.ForceUpdateCanvases();
        }

        private void OnSelectSeed(ItemData seed) {
            if(_currentPlot) _currentPlot.PlantMax(seed);
            Close();
        }
    }
}
