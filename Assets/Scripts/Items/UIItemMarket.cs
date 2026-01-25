using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using Managers;
using Markets;

// ReSharper disable Unity.PerformanceCriticalCodeInvocation
// <summary> Used in prefab as object in market </summary>
namespace Items {
    public class UIItemMarket : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {  // Used in markets
        [Header("Data References")]
        [SerializeField] private MarketOffer offer;
        
        [Header("UI Components")]
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image itemIcon;
        [SerializeField] private Text quantityText;
        [SerializeField] private Text nameText;
        [SerializeField] private Text priceText;
        [SerializeField] private GameObject discountBadge;
        [SerializeField] private Text discountText;

        private InventoryManager _inventory;
        private MarketManager _market;
        private Color _priceColor;

        private int _index;

        public void Init(int index, MarketManager market, InventoryManager inventory, MarketOffer newOffer) {
            _index = index;
            _market = market;
            _inventory = inventory;
            offer = newOffer;
            
            if (priceText) _priceColor = priceText.color;                       // Save default price color text
            if (backgroundImage) backgroundImage.enabled = false;                        // Hide by default

            UpdateUI();

            GetComponent<Button>().onClick.AddListener(OnClick);                // Buy offer on click
        }

        private void UpdateUI() {
            if (offer == null) return;

            if (itemIcon) itemIcon.sprite = offer.item.icon;
            if (quantityText) quantityText.text = "x" + offer.quantity;

            if (nameText) nameText.color = offer.item.GetRarityColor();         // Apply rarity color to name text
            if (nameText) nameText.text = offer.item.name;

            if (priceText) {
                priceText.text = offer.FinalPrice + " $";
                priceText.color = _inventory.Money < offer.FinalPrice ? Color.red : _priceColor;  // Show if can buy product or not
            }

            if (discountBadge) {
                bool hasDiscount = offer.discount > 0;
                discountBadge.gameObject.SetActive(hasDiscount);
                if (hasDiscount && discountText) {
                    discountText.color = offer.GetDiscountColor();
                    discountText.text = offer.GetDiscountText();
                }
            }
        }

        private void OnClick() {
            if (!_market || !_inventory) return;

            bool success = _market.BuyOffer(_index, _inventory);

            if (success) _inventory.UpdateMoneyUI();
            else Debug.Log("Not enough money!");                                // L Display text on screen
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (!offer.item) return;
            if (backgroundImage) backgroundImage.enabled = true;
            UIItemOverlayManager.Instance?.Show(offer.item);
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (backgroundImage) backgroundImage.enabled = false;
            UIItemOverlayManager.Instance?.Hide();
        }

        public void OnDisable() {
            if (backgroundImage) backgroundImage.enabled = false;
            UIItemOverlayManager.Instance?.Hide();
        }
    }
}
