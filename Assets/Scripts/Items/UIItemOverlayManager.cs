using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;

namespace Items {
    public class UIItemOverlayManager : MonoBehaviour {
        [Header("References")]
        public RectTransform overlayRoot;
        public RawImage itemImage;
        public Text typeText;
        public Text buyPriceText;
        public Text sellPriceText;
        public Text descriptionText;

        [Header("Optional fields")]
        public Text nutritionText;
        public Text growTimeText;
        public Text cropNameText;
        public RawImage cropImage;

        [Header("Position setting")]
        public Vector2 mouseOffset = new(40f, -40f); 

        public static UIItemOverlayManager Instance;

        private Canvas _canvas;
        private ItemData _currentItem;

        void Awake() {
            Instance = this;
            _canvas = GetComponentInParent<Canvas>();
            Hide();                                                             // Hide by default
        }

        private void Update() {
            FollowMouse();
        }

        private void FollowMouse() {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform, Mouse.current.position.ReadValue(),
                _canvas.worldCamera, out var pos
            );
            overlayRoot.anchoredPosition = pos + mouseOffset;
        }

        public void Show(ItemData item) {
            if (_currentItem == item) return;

            overlayRoot.gameObject.SetActive(true);
            _currentItem = item;

            // UI Fields
            itemImage.texture = item.icon ? item.icon.texture : null;

            typeText.text = $"Type : {item.type}";
            buyPriceText.text = $"Buy price : {item.buyPrice} $";
            sellPriceText.text = $"Sell price : {item.sellPrice} $";
            descriptionText.text = item.description;

            // Optional fields
            nutritionText.gameObject.SetActive(item.nutritionValue > 0);
            if (item.nutritionValue > 0) nutritionText.text = $"Nutrition : {item.nutritionValue}";

            growTimeText.gameObject.SetActive(item.growTime > 0);
            if (item.growTime > 0) growTimeText.text = $"Grow time : {item.growTime:F1} sec";

            cropNameText.gameObject.SetActive(item.cropProduced);
            if (item.cropProduced) cropImage.texture = item.cropProduced.icon.texture;
        }

        public void Hide() {
            overlayRoot.gameObject.SetActive(false);
            _currentItem = null;
        }
    }
}
