using System.Collections.Generic;
using UnityEngine.InputSystem;                                                  // For Keyboard
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

// ReSharper disable Unity.PerformanceCriticalCodeInvocation
namespace Items {
    public class InventoryManager : MonoBehaviour {
        [Header("Inventory Settings")]
        [SerializeField] private bool isOpened;
        [SerializeField] private Vector2Int inventorySize = new(8, 6);
        [SerializeField] private int money = 500;                               // start with 100$ in real game

        [Header("UI References")]
        [SerializeField] private Button inventoryButton;
        [SerializeField] private GameObject uiInventoryPanel;
        [SerializeField] private ScrollRect inventoryScrollRect;
        [SerializeField] private RectTransform inventoryBgImage;                // To detect click
        [SerializeField] private RectTransform slotsContainer;                  // Grid Layout Group object
        [SerializeField] private RectTransform viewportRect;                    // In ScrollView
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private Text moneyText;

        [Header("Inventory content")]
        public List<ItemInstance> items = new();

        private readonly List<InventorySlot> _uiSlots = new();                  // List of generated slots

        public bool IsOpened => isOpened;
        public int Money => money;

        private void Start() {
            if (!inventoryBgImage && uiInventoryPanel) inventoryBgImage = uiInventoryPanel.GetComponent<RectTransform>();

            if (inventoryButton) {
                inventoryButton.onClick.AddListener(ToggleOpening);
                UpdateIconButton();
            }
            
            UpdateMoneyUI();                                                    // Display player's money
            StartCoroutine(InitInventoryDelay());
        }

        private void Update() {
            if (isOpened && Mouse.current.leftButton.wasPressedThisFrame) CheckClickOutside();
        }

        public void AddMoney(int amount) {
            money += amount;
        }

        private IEnumerator InitInventoryDelay() {
            uiInventoryPanel.SetActive(true);                                   // Activate to get size
            yield return null;                                                  // Wait a frame

            InitInventoryGrid();                                                // Use size for calculation

            if (uiInventoryPanel) uiInventoryPanel.SetActive(isOpened);         // Hide by default
        }

        private void InitInventoryGrid() {
            if (!slotsContainer || !slotPrefab || !viewportRect) return;

            foreach (Transform child in slotsContainer) Destroy(child.gameObject);  // Remove potential children
            _uiSlots.Clear();

            GridLayoutGroup grid = slotsContainer.GetComponent<GridLayoutGroup>();
            if (!grid) return;

            int columns = inventorySize.x;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = columns;                                     // Force to have number of columns

            RectTransform containerRect = slotsContainer.GetComponent<RectTransform>();
            float containerWidth = containerRect.rect.width;

            float padding = grid.padding.horizontal;
            float spacingTotal = grid.spacing.x * (columns - 1);
            float availableWidth = containerWidth - padding - spacingTotal;     // Calculate space available
            if (availableWidth < 0) availableWidth = 100;

            float size = availableWidth / columns;
            grid.cellSize = new Vector2(size, size);

            int totalSlots = inventorySize.x * inventorySize.y;
            for (int i = 0; i < totalSlots; i++) {                              // Create slots
                GameObject slotObj = Instantiate(slotPrefab, slotsContainer);
                var script = slotObj.GetComponent<InventorySlot>();
                _uiSlots.Add(script);
            }
        }

        private void CheckClickOutside() {
            bool isMouseOnInv = RectTransformUtility.RectangleContainsScreenPoint(
                inventoryBgImage, Mouse.current.position.ReadValue(), null);

            if (!isMouseOnInv) ToggleOpening();                               // Close if click outside
        }

        public void ToggleOpening() {
            isOpened = !isOpened;
            
            if (uiInventoryPanel) {
                uiInventoryPanel.SetActive(isOpened);
                if (isOpened) RefreshUI();                                      // Update when open
            }
            
            UpdateIconButton();
        }

        public void ToggleScrolling(bool canScroll) {
            if (inventoryScrollRect) inventoryScrollRect.vertical = canScroll;
        }

        private void RefreshUI() {                                              // Update slots
            for (int i = 0; i < _uiSlots.Count; i++) {
                if (i < items.Count) _uiSlots[i].SetItem(items[i]);             // Display item
                else _uiSlots[i].Clear();                                       // Or display empty slot
            }
        }

        public void UpdateMoneyUI() {
            if (moneyText) moneyText.text = money + " $";
        }

        private void UpdateIconButton() {
            if (inventoryButton) inventoryButton.interactable = !isOpened;      // Can click only to open
        }

        public bool AddItem(ItemData data, int amount = 1) {                    // Manage stack & capacity
            if (data.stackable) {                                               // Make stacks
                foreach (var item in items) {
                    if (item.data == data && item.quantity < data.maxStack) {   // Check if stack isn't full 
                        int spaceInStack = data.maxStack - item.quantity;
                        int toAdd = Mathf.Min(spaceInStack, amount);

                        item.quantity += toAdd;
                        amount -= toAdd;

                        if (amount <= 0) { RefreshUI(); return true; }          // All inventory has been added
                    }
                }
            }

            while (amount > 0) {                                                // Create new pile for not stackable items
                if (items.Count >= inventorySize.x * inventorySize.y) {         // If inventory is full
                    RefreshUI();
                    return false;                                               // Can't add item
                }

                int newStackAmount = Mathf.Min(amount, data.maxStack);
                items.Add(new ItemInstance(data, newStackAmount));
                amount -= newStackAmount;
            }

            RefreshUI();
            return true;
        }

        public void RemoveItem(ItemData data, int amount = 1) {
            for (int i = items.Count - 1; i >= 0; i--) {                        // Go backwards
                if (items[i].data == data) {
                    int toRemove = Mathf.Min(amount, items[i].quantity);
                    items[i].quantity -= toRemove;
                    amount -= toRemove;

                    if (items[i].quantity <= 0) items.RemoveAt(i);

                    if (amount <= 0) break;
                }
            }

            RefreshUI();
        }

        public void SellItem(ItemInstance instance, int amount=0) {
            if (instance == null || !instance.data || amount <= 0) return;

            int totalSellPrice = instance.data.sellPrice * amount;              // Calculate sell price

            money += totalSellPrice;                                            // Sell it
            UpdateMoneyUI();

            instance.quantity -= amount;
            if (instance.quantity <= 0) items.Remove(instance);                 // Remove from inventory if empty
            RefreshUI();                                                        // Update UI
        }
    }
}
