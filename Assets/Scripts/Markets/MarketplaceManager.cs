using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;
using Managers;
using Save;

// ReSharper disable Unity.PerformanceCriticalCodeInvocation
namespace Markets {
    public class MarketplaceManager : MonoBehaviour {
        [Header("References")]
        [Tooltip("Market place object (usually self)")]
        [SerializeField] private RectTransform uiMarketplaceContainer;
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;

        [Header("Settings")]
        [Tooltip("Distance in pixels to slide for each market")]
        [SerializeField] private float slideDistance = 900f;
        [Tooltip("Time in seconds to complete the slide")]
        [SerializeField] private float slideDuration = 0.5f;
        
        [Header("Navigation Data")]
        [Tooltip("Total number of markets aligned")]
        [SerializeField] private int totalMarkets = 3;
        [Tooltip("Index of the market visible at start (0=Left, 1=Middle, etc)")]
        [SerializeField] private int startIndex = 1;

        private int _currentIndex;
        private bool _isMoving;                                                 // Moving between markets
        private bool _isInitialized;

        private void Start() {
            if (!uiMarketplaceContainer) uiMarketplaceContainer = GetComponent<RectTransform>();
            _currentIndex = startIndex;                                         // Init index

            if (leftButton) leftButton.onClick.AddListener(OnLeftButtonClicked);// Config buttons
            if (rightButton) rightButton.onClick.AddListener(OnRightButtonClicked);

            UpdateButtonsState();                                               // Update buttons display
            
            StartCoroutine(InitMarketsRoutine());
        }

        private void Update() {
            if (_isMoving) return;

            if (Keyboard.current != null) {                                     // Also work with arrows
                if (Keyboard.current.leftArrowKey.wasPressedThisFrame) OnLeftButtonClicked();
                else if (Keyboard.current.rightArrowKey.wasPressedThisFrame) OnRightButtonClicked();
            }
        }

        private IEnumerator InitMarketsRoutine() {
            yield return null;                                                  // Wait for child scripts to be initiate

            MarketManager[] markets = uiMarketplaceContainer.GetComponentsInChildren<MarketManager>(true);
            var savedData = GameManager.Instance.currentSave.marketplace;

            for (int i = 0; i < markets.Length; i++) {
                if (savedData != null && i < savedData.Count)
                    markets[i].LoadMarket(savedData[i]);                        // If save exist for this market
                else {                                                          // Ask market to generate
                    markets[i].GenerateMarket(); markets[i].DisplayMarket();
                }
            }

            _isInitialized = true;
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

        private void OnDisable() {
            if (_isInitialized && !GameManager.Instance) return;

            MarketManager[] markets = uiMarketplaceContainer.GetComponentsInChildren<MarketManager>(true);

            List<MarketSaveData> allMarketsData = new List<MarketSaveData>();
            foreach (var m in markets) allMarketsData.Add(m.GetSaveData());
            GameManager.Instance.SyncMarketplace(allMarketsData);
        }
    }
}
