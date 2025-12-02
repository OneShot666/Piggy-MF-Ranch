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
        [Tooltip("Fixed number of rows to display.")]
        public int rowsToDisplay = 2;
        public int maxStackOffer = 10;

        [Header("UI Settings")]
        [Tooltip("RectTransform where to place items")]
        public RectTransform itemsDisplayArea;
        [Tooltip("Prefab UI : icon + price + quantity")]
        public GameObject itemUIPrefab;
        [Tooltip("Space between items (in pixels).")]
        public Vector2 spacing = new(60, 30);
        public float maxItemSize = 100f;

        [Header("Offers (Generated)")]
        public List<MarketOffer> currentOffers = new();

        private readonly List<GameObject> _activeUIItems = new();
        public static event System.Action OnGlobalPurchase;

        private void Start() {
            if (!inventoryUI) inventoryUI = FindFirstObjectByType<InventoryManager>();
            GenerateMarket();
            DisplayMarket();
        }

        private void OnEnable() => OnGlobalPurchase += DisplayMarket;           // Listen to signal (buying item)

        private void OnDisable() => OnGlobalPurchase -= DisplayMarket;          // Stop listening

        [ContextMenu("Generate Market")]
        private void GenerateMarket() {
            if (!itemPool || itemPool.possibleItems.Length == 0) return;

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
            OnGlobalPurchase?.Invoke();                                         // Update all markets when buy something

            return true;
        }

        private void DisplayMarket() {                                          // UI Display
            foreach (var uiObj in _activeUIItems) Destroy(uiObj);               // Clear previous UI objects
            _activeUIItems.Clear();

            if (!itemsDisplayArea || !itemUIPrefab) return;

            int count = currentOffers.Count;
            if (count == 0) return;

            GridLayoutGroup grid = itemsDisplayArea.GetComponent<GridLayoutGroup>();
            if (!grid) grid = itemsDisplayArea.gameObject.AddComponent<GridLayoutGroup>();
            
            grid.spacing = spacing;                                             // Apply spacing

            int targetRows = Mathf.Clamp(rowsToDisplay, 1, count);
            int columns = Mathf.CeilToInt((float)count / targetRows);           // Get number of items per columns
            int rows = Mathf.CeilToInt((float)count / columns);                 // Get actual number of items per rows

            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;      // Force to respect this number of columns
            grid.constraintCount = columns;

            float rectWidth = itemsDisplayArea.rect.width;                      // Get dynamic item size
            float rectHeight = itemsDisplayArea.rect.height;

            float widthAvailable = rectWidth - grid.padding.horizontal - grid.spacing.x * (columns - 1);    // Adapt spacing
            float heightAvailable = rectHeight - grid.padding.vertical - grid.spacing.y * (rows - 1);
            if (widthAvailable < 0) widthAvailable = 0;
            if (heightAvailable < 0) heightAvailable = 0;

            float sizePerCellX = widthAvailable / columns;
            float sizePerCellY = heightAvailable / rows;
            float cellSize = Mathf.Min(sizePerCellX, sizePerCellY);             // Keep square item image
            cellSize = Mathf.Min(cellSize, maxItemSize);                        // Limit size of items

            grid.cellSize = new Vector2(cellSize, cellSize);
            grid.childAlignment = TextAnchor.MiddleCenter;                      // Center items

            for (int i = 0; i < currentOffers.Count; i++) {
                GameObject ui = Instantiate(itemUIPrefab, itemsDisplayArea);
                _activeUIItems.Add(ui);

                var uiItem = ui.GetComponent<MarketUIItem>();
                if (uiItem) uiItem.Init(i, this, inventoryUI);

                RectTransform rt = ui.GetComponent<RectTransform>();
                rt.anchorMin = rt.anchorMax = new Vector2(0, 1);
                rt.pivot = new Vector2(0, 1);

                // Assign item data
                var offer = currentOffers[i];
                var slot = ui.GetComponent<UIItemSlot>();
                if (slot) slot.Init(offer.item, offer.quantity);
                ui.transform.Find("ItemImage").GetComponent<Image>().sprite = offer.item.icon;
                ui.transform.Find("QuantityText").GetComponent<Text>().text = "x" + offer.quantity;

                var nameText = ui.transform.Find("NameText").GetComponent<Text>();
                nameText.color = offer.item.GetRarityColor();                   // Apply rarity color to name text
                nameText.text = offer.item.name;

                Text priceText = ui.transform.Find("PriceText").GetComponent<Text>();
                priceText.text = offer.FinalPrice + " $";
                if (inventoryUI.money < offer.FinalPrice) priceText.color = Color.red;  // Show if can buy product or not
            }
        }
    }
}
