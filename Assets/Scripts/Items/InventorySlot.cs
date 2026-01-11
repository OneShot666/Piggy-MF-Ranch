using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

namespace Items {
    public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
        [SerializeField] private ItemData item;
        [SerializeField] private Image iconImage;
        [SerializeField] private Text quantityText;

        private InventoryManager _manager;
        private ItemInstance _instance;

        public void Clear() {                                                   // Reset slot
            iconImage.sprite = null;
            iconImage.enabled = false;                                          // Hide default image if empty
            if (quantityText) quantityText.text = "";
        }

        public void SetItem(ItemInstance newItem) {
            if (newItem == null || !newItem.data) { Clear(); return; }

            _manager = FindFirstObjectByType<InventoryManager>();
            _instance = newItem;
            item = newItem.data;
            iconImage.sprite = newItem.data.icon;
            iconImage.enabled = true;
            if (quantityText) quantityText.text = newItem.quantity > 1 ? $"x{newItem.quantity}" : ""; // Display if more than 1
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (_instance == null || !UISellOverlay.Instance) return;

            if (eventData.button == PointerEventData.InputButton.Left) {        // If click on slot
                if (UISellOverlay.Instance.IsActive()) {                        // If click again, sell current quantity
                    int amount = UISellOverlay.Instance.SelectedQuantity;
                    if (_manager) _manager.SellItem(_instance, amount);
                    UISellOverlay.Instance.Hide();
                } else {                                                        // Else show sell overlay
                    UIItemOverlayManager.Instance?.Hide();                      // Hide item overlay
                    UISellOverlay.Instance.Show(_instance, transform.position);
                }
            }

            if (UISellOverlay.Instance.IsActive() && eventData.button == PointerEventData.InputButton.Right) {
                if (_manager) _manager.SellItem(_instance, _instance.quantity); // Sell the whole slot
                UIItemOverlayManager.Instance?.Hide();                          // Hide item overlay
                UISellOverlay.Instance?.Hide();                                 // Hide sell overlay
            }
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (!item) return;
            UIItemOverlayManager.Instance?.Show(item);
        }

        public void OnPointerExit(PointerEventData eventData) {
            UIItemOverlayManager.Instance?.Hide();
            UISellOverlay.Instance?.Hide();
        }

        public void OnDisable() {
            UIItemOverlayManager.Instance?.Hide();
        }
    }
}
