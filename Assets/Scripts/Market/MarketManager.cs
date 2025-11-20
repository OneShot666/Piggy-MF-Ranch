using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Items;

namespace Market {
    public class MarketManager : MonoBehaviour {

        [Header("Market Settings")]
        public MarketItemPool itemPool;
        public int stallSize = 6;

        [Header("UI Settings")]
        [Tooltip("Zone RectTransform où placer les items à vendre sur l'image du marché.")]
        public RectTransform itemsDisplayArea;

        [Tooltip("Prefab UI : icon + price + quantity")]
        public GameObject itemUIPrefab;

        [Tooltip("Space between items (en pixels).")]
        public float spacing = 10f;

        [Header("Generated Offers (Runtime)")]
        public List<MarketOffer> currentOffers = new();

        private readonly List<GameObject> _activeUIItems = new();

        private void Start() {
            GenerateMarket();
            DisplayMarket();
        }

        [ContextMenu("Generate Market")]
        private void GenerateMarket() {
            currentOffers.Clear();

            if (!itemPool || itemPool.possibleItems.Length == 0) {
                Debug.LogWarning("Market item pool is empty!");
                return;
            }

            for (int i = 0; i < stallSize; i++) {
                ItemData item = itemPool.possibleItems[
                    Random.Range(0, itemPool.possibleItems.Length)
                ];

                MarketOffer offer = new MarketOffer {
                    item = item,
                    quantity = Random.Range(1, 11)
                };

                offer.basePrice = item.buyPrice * offer.quantity;
                offer.discount = Random.value < 0.3f ? Random.Range(0.10f, 0.50f) : 0f;
                currentOffers.Add(offer);
            }
        }

        public bool BuyOffer(int index, Inventory inventory) {
            if (index < 0 || index >= currentOffers.Count) return false;

            MarketOffer offer = currentOffers[index];

            if (inventory.money < offer.FinalPrice) return false;

            inventory.money -= offer.FinalPrice;
            inventory.AddItem(offer.item, offer.quantity);

            currentOffers.RemoveAt(index);
            DisplayMarket();

            return true;
        }

        private void DisplayMarket() {                                          // UI Display
            foreach (var uiObj in _activeUIItems) Destroy(uiObj);               // Clear previous UI objects
            _activeUIItems.Clear();

            if (!itemsDisplayArea || !itemUIPrefab) return;

            // Simple grid layout
            float width = itemsDisplayArea.rect.width;
            float height = itemsDisplayArea.rect.height;

            int count = currentOffers.Count;
            int columns = Mathf.CeilToInt(Mathf.Sqrt(count));
            int rows = Mathf.CeilToInt(count / (float)columns);

            float cellW = (width - spacing * (columns - 1)) / columns;
            float cellH = (height - spacing * (rows - 1)) / rows;

            for (int i = 0; i < count; i++) {
                int row = i / columns;
                int col = i % columns;

                GameObject ui = Instantiate(itemUIPrefab, itemsDisplayArea);
                _activeUIItems.Add(ui);

                RectTransform rt = ui.GetComponent<RectTransform>();
                rt.anchorMin = rt.anchorMax = new Vector2(0, 1);
                rt.pivot = new Vector2(0, 1);

                float x = col * (cellW + spacing);
                float y = -row * (cellH + spacing);

                rt.anchoredPosition = new Vector2(x, y);
                rt.sizeDelta = new Vector2(cellW, cellH);

                // Assign item data
                var offer = currentOffers[i];
                ui.transform.Find("ItemImage").GetComponent<Image>().sprite = offer.item.icon;
                ui.transform.Find("NameQuantityText").GetComponent<Text>().text = offer.item.name + "(x" + offer.quantity + ")";
                ui.transform.Find("PriceText").GetComponent<Text>().text = offer.FinalPrice + " $";
            }
        }
    }
}
