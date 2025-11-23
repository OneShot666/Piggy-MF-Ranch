using UnityEngine.EventSystems;
using UnityEngine;

namespace Items {
    public class UIItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        public ItemData item;
        public int quantity;

        public void Init(ItemData currentItem, int currentQuantity=1) {
            item = currentItem;
            quantity = currentQuantity;
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (!item) return;
            UIItemOverlayManager.Instance?.Show(item);
        }

        public void OnPointerExit(PointerEventData eventData) {
            UIItemOverlayManager.Instance?.Hide();
        }
    }
}
