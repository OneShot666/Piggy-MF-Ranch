using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace Market {
    public class MarketplaceManager : MonoBehaviour {
        [Header("References")]
        [Tooltip("Market place object (usually self)")]
        public RectTransform uiMarketplaceContainer;
        public Button leftButton;
        public Button rightButton;

        [Header("Settings")]
        [Tooltip("Distance in pixels to slide for each market")]
        public float slideDistance = 900f;
        [Tooltip("Time in seconds to complete the slide")]
        public float slideDuration = 0.5f;
        
        [Header("Navigation Data")]
        [Tooltip("Total number of markets aligned")]
        public int totalMarkets = 3;
        [Tooltip("Index of the market visible at start (0=Left, 1=Middle, etc)")]
        public int startIndex = 1;

        private int _currentIndex;
        private bool _isMoving;

        private void Start() {
            _currentIndex = startIndex;                                         // Init index

            if (leftButton) leftButton.onClick.AddListener(OnLeftButtonClicked);    // Config buttons
            if (rightButton) rightButton.onClick.AddListener(OnRightButtonClicked);

            UpdateButtonsState();                                               // Update buttons display
        }

        private void Update() {
            if (_isMoving) return;

            if (Keyboard.current != null) {                                     // Also work with arrows
                if (Keyboard.current.leftArrowKey.wasPressedThisFrame) OnLeftButtonClicked();
                else if (Keyboard.current.rightArrowKey.wasPressedThisFrame) OnRightButtonClicked();
            }
        }

        private void OnLeftButtonClicked() {
            if (_isMoving) return;
            if (_currentIndex > 0) MoveToIndex(_currentIndex - 1);
        }

        private void OnRightButtonClicked() {
            if (_isMoving) return;
            if (_currentIndex < totalMarkets - 1) MoveToIndex(_currentIndex + 1);
        }

        private void MoveToIndex(int targetIndex) {
            float targetX = (startIndex - targetIndex) * slideDistance;         // Check index and move
            Vector2 targetPos = new Vector2(targetX, uiMarketplaceContainer.anchoredPosition.y);

            StartCoroutine(SlideRoutine(targetPos, targetIndex));
        }

        private IEnumerator SlideRoutine(Vector2 targetPosition, int targetIndex) {
            Vector2 startPosition = uiMarketplaceContainer.anchoredPosition;
            float elapsedTime = 0f;
            _isMoving = true;

            while (elapsedTime < slideDuration) {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / slideDuration;
                t = t * t * (3f - 2f * t);                                      // Make smooth movement

                uiMarketplaceContainer.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
                yield return null;
            }

            uiMarketplaceContainer.anchoredPosition = targetPosition;           // Get exact final position
            _currentIndex = targetIndex;
            _isMoving = false;
            
            UpdateButtonsState();
        }

        private void UpdateButtonsState() {
            if (leftButton) leftButton.gameObject.SetActive(_currentIndex > 0);
            if (rightButton) rightButton.gameObject.SetActive(_currentIndex < totalMarkets - 1);
        }

        private void OnDestroy() {                                              // Clean events if self is destroy
            if (leftButton) leftButton.onClick.RemoveListener(OnLeftButtonClicked);
            if (rightButton) rightButton.onClick.RemoveListener(OnRightButtonClicked);
        }
    }
}
