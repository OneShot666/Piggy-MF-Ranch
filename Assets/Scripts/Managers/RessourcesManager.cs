using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Pigs;

namespace Managers {
    public class RessourcesManager : MonoBehaviour {
        [Header("References")]
        [SerializeField] private PigManager pigManager;
        [SerializeField] private RectTransform menuRect;
        [SerializeField] private Text toggleButtonText;

        [Header("Stats Sliders")]
        [SerializeField] private Slider happinessSlider;
        [SerializeField] private Slider hungerSlider;
        [SerializeField] private Slider cleanlinessSlider;
        [SerializeField] private Slider enduranceSlider;

        [Header("Settings")]
        [SerializeField] private bool openByDefault;
        [SerializeField] private float slideSpeed = 5f;

        private TextMeshProUGUI _happinessText;
        private TextMeshProUGUI _hungerText;
        private TextMeshProUGUI _cleanlinessText;
        private TextMeshProUGUI _enduranceText;
        private Vector2 _openPos;
        private Vector2 _closedPos;
        private Vector2 _targetPos;
        private bool _isOpen;

        private void Start() {
            AutoFindPositions();
            AutoFindSliderTexts();
        }

        private void AutoFindPositions() {
            float menuHeight = menuRect.rect.height;                            // Animation is based on height

            if (openByDefault) {
                _openPos = menuRect.anchoredPosition;                           // Get current position
                _closedPos = _openPos + new Vector2(0, menuHeight);             // Calculate closed position
            } else {
                _closedPos = menuRect.anchoredPosition;
                _openPos = _closedPos - new Vector2(0, menuHeight);             // Calculate opened position
            }
            _targetPos = menuRect.anchoredPosition;
        }

        private void AutoFindSliderTexts() {
            _happinessText =   happinessSlider.GetComponentInChildren<TextMeshProUGUI>();
            _hungerText =      hungerSlider.GetComponentInChildren<TextMeshProUGUI>();
            _cleanlinessText = cleanlinessSlider.GetComponentInChildren<TextMeshProUGUI>();
            _enduranceText =   enduranceSlider.GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Update() {                                                 // Smooth sliding animation
            menuRect.anchoredPosition = Vector2.Lerp(menuRect.anchoredPosition, _targetPos, Time.deltaTime * slideSpeed);
            // menuRect.anchoredPosition = Vector2.MoveTowards(menuRect.anchoredPosition, _targetPos, (slideSpeed * 100) * Time.deltaTime);

            UpdatePigStats();                                                   // Update slider
        }

        public void ToggleMenu() {                                              // Used by toggle button
            _isOpen = !_isOpen;
            _targetPos = _isOpen ? _openPos : _closedPos;

            if (toggleButtonText) toggleButtonText.transform.localScale = new Vector3(1, _isOpen ? -1f : 1f, 1);  // Invert text
        }

        private void UpdatePigStats() {
            if (!pigManager) return;

            Pig currentPig = pigManager.GetCurrentPig();

            if (currentPig != null) {                                           // Display pig stats (if any selected)
                if (happinessSlider) {
                    happinessSlider.value = currentPig.Happiness;
                    happinessSlider.maxValue = currentPig.HappinessMax;
                    if (_happinessText) _happinessText.text = $"{currentPig.GetHappinessPercent()}%";
                }
                if (hungerSlider) {
                    hungerSlider.value = currentPig.Hunger;
                    hungerSlider.maxValue = currentPig.HungerMax;
                    if (_hungerText) _hungerText.text = $"{currentPig.GetHungerPercent()}%";
                }
                if (cleanlinessSlider) {
                    cleanlinessSlider.value = currentPig.Cleanliness;
                    cleanlinessSlider.maxValue = currentPig.CleanlinessMax;
                    if (_cleanlinessText) _cleanlinessText.text = $"{currentPig.GetCleanlinessPercent()}%";
                }
                if (enduranceSlider) {
                    enduranceSlider.value = currentPig.Endurance;
                    enduranceSlider.maxValue = currentPig.EnduranceMax;
                    if (_enduranceText) _enduranceText.text = $"{currentPig.GetEndurancePercent()}%";
                }
            }
        }
    }
}
