using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Items;

// ReSharper disable Unity.PerformanceCriticalCodeInvocation
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

        [Header("Refresh Settings")]
        [Tooltip("Button to refresh offers")]
        public Button refreshButton;
        [Tooltip("Text inside the button to show cost")]
        public Text refreshCostText;
        [Tooltip("Cost for the first paid refresh")]
        public int baseRefreshCost = 5; 
        [Tooltip("How much the cost increases each time")]
        public int costIncrement = 5;

        [Header("UI Settings")]
        [Tooltip("If true, odd rows are shifted right and even rows left.")]
        public bool useStaggeredLayout;
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
        private bool _shouldUpdateUI;                                           // For update on scene when changed in editor
        private int _currentRefreshCost;                                        // First one is free

        private System.Collections.IEnumerator Start() {
            if (!inventoryUI) inventoryUI = FindFirstObjectByType<InventoryManager>();

            if (refreshButton) {
                refreshButton.onClick.AddListener(TryRefreshMarket);
                UpdateRefreshUI();
            }
            
            yield return null;                                                  // Wait a frame

            GenerateMarket();
            DisplayMarket();
        }

        private void Update() {
            if (_shouldUpdateUI) {
                DisplayMarket();
                _shouldUpdateUI = false;
            }
        }

        private void OnEnable() => OnGlobalPurchase += DisplayMarket;           // Listen to signal (buying item)

        private void OnDisable() => OnGlobalPurchase -= DisplayMarket;          // Stop listening

        private void OnValidate() {
            if (Application.isPlaying) _shouldUpdateUI = true;
        }

        [ContextMenu("Refresh market")]
        private void TryRefreshMarket() {
            if (inventoryUI.money < _currentRefreshCost) return;                // If don't have enough money

            inventoryUI.money -= _currentRefreshCost;                           // Pay refresh
            inventoryUI.UpdateMoneyUI();

            if (_currentRefreshCost == 0) _currentRefreshCost = baseRefreshCost;// Increase refresh price
            else _currentRefreshCost += costIncrement;

            GenerateMarket();
            DisplayMarket();

            UpdateRefreshUI();                                                  // Update button
            OnGlobalPurchase?.Invoke(); 
        }

        private void UpdateRefreshUI() {
            if (!refreshCostText) return;

            if (_currentRefreshCost == 0) {
                refreshCostText.text = "Free";
                refreshCostText.color = Color.green;
            } else {
                refreshCostText.text = $"{_currentRefreshCost} $";
                bool canAfford = inventoryUI.money >= _currentRefreshCost;      // Check if enough money
                refreshCostText.color = canAfford ? Color.white : Color.red;
            }
        }

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
                        var pick = itemPool.possibleItems[Random.Range(0, itemPool.possibleItems.Length)];
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
                    int step = Random.Range(2, 19);                             // Between -10% & -90%
                    offer.discount = step * 0.05f;                              // Step of 5%
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
            Canvas.ForceUpdateCanvases();                                       // Update canvas size immediately

            foreach (var uiObj in _activeUIItems) Destroy(uiObj);               // Clear previous UI objects
            _activeUIItems.Clear();

            if (!itemsDisplayArea || !itemUIPrefab) return;

            int count = currentOffers.Count;
            if (count == 0) return;

            GridLayoutGroup grid = itemsDisplayArea.GetComponent<GridLayoutGroup>();
            if (useStaggeredLayout && grid) DestroyImmediate(grid);
            else if (!grid) grid = itemsDisplayArea.gameObject.AddComponent<GridLayoutGroup>();

            int targetRows = Mathf.Clamp(rowsToDisplay, 1, count);
            int columns = Mathf.CeilToInt((float)count / targetRows);           // Get number of items per columns
            int rows = Mathf.CeilToInt((float)count / columns);                 // Get actual number of items per rows

            float rectWidth = itemsDisplayArea.rect.width;                      // Get dynamic item size
            float rectHeight = itemsDisplayArea.rect.height;

            float widthAvailable = rectWidth - grid.spacing.x * (columns - 1);  // Adapt spacing
            float heightAvailable = rectHeight - grid.spacing.y * (rows - 1);
            if (grid) { widthAvailable -= grid.padding.horizontal; widthAvailable -= grid.padding.vertical; }
            if (widthAvailable < 0) widthAvailable = 0;
            if (heightAvailable < 0) heightAvailable = 0;

            float sizePerCellX = widthAvailable / columns;
            float sizePerCellY = heightAvailable / rows;
            float cellSize = Mathf.Min(sizePerCellX, sizePerCellY);             // Keep square item image
            cellSize = Mathf.Min(cellSize, maxItemSize);                        // Limit size of items

            if (!useStaggeredLayout && grid) {
                grid.spacing = spacing;                                         // Apply spacing
                grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;  // Force to respect number of columns
                grid.constraintCount = columns;
                grid.cellSize = new Vector2(cellSize, cellSize);
                grid.childAlignment = TextAnchor.MiddleCenter;                  // Center items
            }

            float startX = 0f;
            float startY = 0f;

            if (useStaggeredLayout) {
                if (rowsToDisplay <= 1) useStaggeredLayout = false;             // Don't need complicate math for one row
                else {
                    float totalGridWidth = columns * cellSize + (columns - 1) * spacing.x;
                    float totalGridHeight = rows * cellSize + (rows - 1) * spacing.y;
                    startX = (rectWidth - totalGridWidth) / 2;
                    startY = -(rectHeight - totalGridHeight) / 2;
                }
            }

            for (int i = 0; i < currentOffers.Count; i++) {
                GameObject ui = Instantiate(itemUIPrefab, itemsDisplayArea);
                _activeUIItems.Add(ui);

                var uiItem = ui.GetComponent<UIItemMarket>();
                if (uiItem) uiItem.Init(i, this, inventoryUI);

                RectTransform rt = ui.GetComponent<RectTransform>();
                rt.anchorMin = rt.anchorMax = new Vector2(0, 1);                // Automatic positioning
                rt.pivot = new Vector2(0, 1);

                if (useStaggeredLayout) {                                       // Manual positioning
                    int row = i / columns;
                    int col = i % columns;

                    rt.sizeDelta = new Vector2(cellSize, cellSize);

                    float posX = startX + cellSize * 0.25f + col * (cellSize + spacing.x) * 0.9f;
                    float posY = startY - row * (cellSize + spacing.y) * 0.8f;

                    float shiftAmount = cellSize / 2f;                          // Items offset
                    if (row % 2 == 0) posX -= shiftAmount;
                    else posX += shiftAmount;

                    rt.anchoredPosition = new Vector2(posX, posY);
                }

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
                
                Transform discountTransform = ui.transform.Find("DiscountImage");
                if (discountTransform) {
                    bool hasDiscount = offer.discount > 0;
                    discountTransform.gameObject.SetActive(hasDiscount);
                    if (hasDiscount) {
                        Text discountText = discountTransform.Find("DiscountText").GetComponent<Text>();
                        if (discountText) {
                            discountText.color = offer.GetDiscountColor();
                            discountText.text = offer.GetDiscountText();
                        }
                    }
                }
            }

            UpdateRefreshUI();
        }
    }
}
