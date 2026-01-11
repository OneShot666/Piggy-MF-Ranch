using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;

// <summary> Manage overlay UI of items on market when mouse is hover one.
// Used in UI Canvas. <summary>
namespace Items {
    public class UIItemOverlayManager : MonoBehaviour {
        [Header("References")]
        [SerializeField] private RectTransform overlayRoot;
        [SerializeField] private Image itemImage;
        [SerializeField] private Text nameText;
        [SerializeField] private Text typeText;
        [SerializeField] private Text buyPriceText;
        [SerializeField] private Text sellPriceText;
        [SerializeField] private Text descriptionText;

        [Header("Optional fields")]
        [SerializeField] private Text nutritionText;
        [SerializeField] private Text growTimeText;
        [SerializeField] private Text cropNameText;
        [SerializeField] private Image cropImage;

        [Header("Position setting")]
        [SerializeField] private Vector2 mouseOffset = new(145, 55); 

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
            RectTransform canvasRect = _canvas.transform as RectTransform;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, 
                Mouse.current.position.ReadValue(), cam, out var localMousePos);// Get local mouse pos

            Vector2 targetPos = localMousePos + mouseOffset;

            if (canvasRect) {
                Vector2 size = overlayRoot.rect.size;                           // Get overlay screen data
                Vector2 pivot = overlayRoot.pivot;

                float minX = canvasRect.rect.xMin + size.x * pivot.x;           // Get canvas size
                float maxX = canvasRect.rect.xMax - size.x * (1 - pivot.x);
                float minY = canvasRect.rect.yMin + size.y * pivot.y;
                float maxY = canvasRect.rect.yMax - size.y * (1 - pivot.y);

                targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);             // Keep overlay fully on screen
                targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
            }

            overlayRoot.anchoredPosition = targetPos;                           // Place on final position
        }

        public bool IsActive() {
            return transform.gameObject.activeSelf;
        }

        public void Show(ItemData item) {
            if (_currentItem == item && overlayRoot.gameObject.activeSelf) return;

            overlayRoot.gameObject.SetActive(true);
            _currentItem = item;

            // UI Fields
            itemImage.sprite = item.icon;
            itemImage.gameObject.SetActive(item.icon);
            
            nameText.color = item.GetRarityColor();                             // Apply rarity color to name
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
