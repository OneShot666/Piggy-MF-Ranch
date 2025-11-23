using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Items;

namespace Market {
    public class MarketManager : MonoBehaviour {
        [Header("References")]
        public InventoryManager inventoryUI;

        [Header("Market Settings")]
        public MarketItemPool itemPool;
        public bool avoidDuplicates = true;
        [Tooltip("Quantity of item available in this market")]
        public int stallSize = 6;
        public int maxStackOffer = 10;

        [Header("UI Settings")]
        [Tooltip("RectTransform where to place items")]
        public RectTransform itemsDisplayArea;
        [Tooltip("Prefab UI : icon + price + quantity")]
        public GameObject itemUIPrefab;
        [Tooltip("Space between items (in pixels).")]
        public float spacing = 10f;

        [Header("Offers (Generated)")]
        public List<MarketOffer> currentOffers = new();

        private readonly List<GameObject> _activeUIItems = new();

        private void Start() {
            if (!inventoryUI) inventoryUI = FindFirstObjectByType<InventoryManager>();
            GenerateMarket();
            DisplayMarket();
        }

        [ContextMenu("Generate Market")]
        private void GenerateMarket() {
            if (!itemPool || itemPool.possibleItems.Length == 0) {
                Debug.LogWarning("Market item pool is empty!");
                return;
            }

            currentOffers.Clear();
            HashSet<ItemData> usedItems = new HashSet<ItemData>();

            for (int i = 0; i < stallSize; i++) {
                ItemData item = null;

                if (avoidDuplicates) {                                          // Try to find unused item
                    int safe = 100;                                             // Avoid inf loop
                    while (safe-- > 0) {
                        var pick = itemPool.possibleItems[
                            Random.Range(0, itemPool.possibleItems.Length)
                        ];
                        if (!usedItems.Contains(pick)) {
                            item = pick;
                            usedItems.Add(item);
                            break;
                        }
                    }
                    if (!item) break;                                           // Not enough items
                } else {                                                        // Classic version (enable duplicates)
                    item = itemPool.possibleItems[Random.Range(0, itemPool.possibleItems.Length)];
                }

                MarketOffer offer = new MarketOffer {                           // Create offer
                    item = item,
                    quantity = Random.Range(1, maxStackOffer + 1)
                };

                offer.basePrice = item.buyPrice * offer.quantity;
                if (Random.value < 0.3f) {                                      // 30% of discount
                    float discountPercent = Random.Range(0.10f, 0.50f);         // Between -10% & -50%
                    offer.discount = discountPercent;
                } else offer.discount = 0f;

                currentOffers.Add(offer);
            }
        }

        public bool BuyOffer(int index, InventoryManager inventory) {
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

                var uiItem = ui.GetComponent<MarketUIItem>();
                if (uiItem) uiItem.Init(i, this, inventoryUI);

                RectTransform rt = ui.GetComponent<RectTransform>();
                rt.anchorMin = rt.anchorMax = new Vector2(0, 1);
                rt.pivot = new Vector2(0, 1);

                float x = col * (cellW + spacing);
                float y = -row * (cellH + spacing);

                rt.anchoredPosition = new Vector2(x, y);
                float cellSize = Mathf.Min(cellW, cellH);
                rt.sizeDelta = new Vector2(cellSize, cellSize);

                // Assign item data
                var offer = currentOffers[i];
                var slot = ui.GetComponent<UIItemSlot>();
                slot.Init(offer.item, offer.quantity);
                ui.transform.Find("ItemImage").GetComponent<Image>().sprite = offer.item.icon;
                ui.transform.Find("NameQuantityText").GetComponent<Text>().text = offer.item.name + " (x" + offer.quantity + ")";
                Text priceText = ui.transform.Find("PriceText").GetComponent<Text>();
                priceText.text = offer.FinalPrice + " $";
                if (inventoryUI.money < offer.FinalPrice) priceText.color = Color.red;  // Show if can buy product or not
            }
        }
    }
}
