using Managers;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;

#pragma warning disable CS0414 // Field is assigned but its value is never used
namespace Items {
    public class UISellOverlay : MonoBehaviour {
        [SerializeField] private Text sellInfoText;
        [SerializeField] private Vector2 mouseOffset = new(100, 50); 

        private ItemInstance _targetInstance;
        private InventoryManager _manager;
        private RectTransform _root;
        private Canvas _parentCanvas;

        private int _quantityToSell = 1;

        public static UISellOverlay Instance;
        
        public int SelectedQuantity => _quantityToSell;

        void Awake() {
            Instance = this;

            _manager = FindFirstObjectByType<InventoryManager>();
            Canvas[] allCanvases = GetComponentsInParent<Canvas>();             // To avoid getting self canvas
            _parentCanvas = allCanvases.Length > 1 ? allCanvases[^1] : GetComponentInParent<Canvas>();
            _root = GetComponent<RectTransform>();

            Hide();                                                             // Hide by default
        }

        private void Update() {
            if (!IsActive()) return;

            FollowMouse();
            HandleScroll();
        }

        private void FollowMouse() {
            Camera cam = _parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _parentCanvas.worldCamera;
            RectTransform canvasRect = _parentCanvas.transform as RectTransform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, 
                Mouse.current.position.ReadValue(), cam, out var pos);// Get local mouse pos

            Vector2 targetPos = pos + mouseOffset;

            if (canvasRect) {
                Vector2 size = _root.rect.size;                                 // Get overlay screen data
                Vector2 pivot = _root.pivot;

                float minX = canvasRect.rect.xMin + size.x * pivot.x;           // Get canvas size
                float maxX = canvasRect.rect.xMax - size.x * (1 - pivot.x);
                float minY = canvasRect.rect.yMin + size.y * pivot.y;
                float maxY = canvasRect.rect.yMax - size.y * (1 - pivot.y);

                targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);             // Keep overlay fully on screen
                targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
            }

            _root.anchoredPosition = targetPos;                                 // Place on final position
        }

        private void HandleScroll() {
            float scroll = Mouse.current.scroll.ReadValue().y;
            if (scroll == 0 || _targetInstance == null) return;

            if (scroll > 0) _quantityToSell++;                                  // Modify quantity with mouse scroll
            else _quantityToSell--;

            _quantityToSell = Mathf.Clamp(_quantityToSell, 1, _targetInstance.quantity);
            UpdateText();
        }

        private void UpdateText() {
            int totalPrice = _targetInstance.data.sellPrice * _quantityToSell;
            sellInfoText.text = $"Sell x{_quantityToSell} {_targetInstance.data.itemName} for {totalPrice}$ ?";
        }

        public bool IsActive() {
            return _root.gameObject.activeSelf;
        }

        public void Show(ItemInstance instance, Vector3 position) {
            if (instance == null) return;

            if (_manager) _manager.ToggleScrolling(false);

            _root.gameObject.SetActive(true);
            _root.position = position;                                 // Place next to slot

            _targetInstance = instance;
            _quantityToSell = 1;
            
            UpdateText();
        }

        public void Hide() {
            if (_manager) _manager.ToggleScrolling(true);

            if (this && _root) _root.gameObject.SetActive(false);
            _targetInstance = null;
        }
    }
}
