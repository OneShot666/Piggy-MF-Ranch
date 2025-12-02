using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;

// <summary> Manage overlay UI of items on market when mouse is hover one.
// Used in UI Canvas. <summary>
namespace Items {
    public class UIItemOverlayManager : MonoBehaviour {
        [Header("References")]
        public RectTransform overlayRoot;
        public Image itemImage;
        public Text nameText;
        public Text typeText;
        public Text buyPriceText;
        public Text sellPriceText;
        public Text descriptionText;

        [Header("Optional fields")]
        public Text nutritionText;
        public Text growTimeText;
        public Text cropNameText;
        public Image cropImage;

        [Header("Position setting")]
        public Vector2 mouseOffset = new(145, 55); 

        public static UIItemOverlayManager Instance;
        private ItemData _currentItem;
        private Canvas _canvas;

        void Awake() {
            Instance = this;
            _canvas = GetComponentInParent<Canvas>();

            var group = overlayRoot.GetComponent<CanvasGroup>();
            if (!group) group = overlayRoot.gameObject.AddComponent<CanvasGroup>();
            group.alpha = 1f;
            group.blocksRaycasts = false;                                       // To avoid blinking
            group.interactable = false;

            Hide();                                                             // Hide by default
        }
 
        private void Update() {
            if (overlayRoot.gameObject.activeSelf) FollowMouse();
        }

        private void FollowMouse() {
            Camera cam = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, 
                Mouse.current.position.ReadValue(), cam, out var pos);
            overlayRoot.anchoredPosition = pos + mouseOffset;
        }

        public void Show(ItemData item) {
            if (_currentItem == item && overlayRoot.gameObject.activeSelf) return;

            overlayRoot.gameObject.SetActive(true);
            _currentItem = item;

            // UI Fields
            itemImage.sprite = item.icon;
            itemImage.gameObject.SetActive(item.icon);
            
            nameText.color = item.GetRarityColor();                             // Apply rarity color
            nameText.text = item.name;

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
            cropImage.gameObject.SetActive(item.cropProduced);
            if (item.cropProduced) cropImage.sprite = item.cropProduced.icon;
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(overlayRoot);
        }

        public void Hide() {
            overlayRoot.gameObject.SetActive(false);
            _currentItem = null;
        }
    }
}
