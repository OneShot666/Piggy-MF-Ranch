using UnityEngine.UI;
using UnityEngine;

namespace Items {
    public class InventorySlot : MonoBehaviour {
        public Image iconImage;
        public Text quantityText;

        public void Clear() {                                                   // Reset slot
            iconImage.sprite = null;
            iconImage.enabled = false;                                          // Hide default image if empty
            if (quantityText) quantityText.text = "";
        }

        public void SetItem(ItemInstance item) {
            if (item == null || !item.data) { Clear(); return; }

            iconImage.sprite = item.data.icon;
            iconImage.enabled = true;
            if (quantityText) quantityText.text = item.quantity > 1 ? $"x{item.quantity}" : ""; // Display if more than 1
        }
    }
}
