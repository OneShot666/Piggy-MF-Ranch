using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Pigs;

namespace Managers {
    public class RessourcesManager : MonoBehaviour {
        [Header("References")]
        [SerializeField] private EnclosureManager enclosureManager;
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
            if (!enclosureManager) enclosureManager = FindFirstObjectByType<EnclosureManager>();
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
            // C Alternative animation line
            // menuRect.anchoredPosition = Vector2.MoveTowards(menuRect.anchoredPosition, _targetPos, (slideSpeed * 100) * Time.deltaTime);

            UpdatePigStats();                                                   // Update slider
        }

        public void ToggleMenu() {                                              // Used by toggle button
            _isOpen = !_isOpen;
            _targetPos = _isOpen ? _openPos : _closedPos;

            if (toggleButtonText) toggleButtonText.transform.localScale = new Vector3(1, _isOpen ? -1f : 1f, 1);  // Invert text
        }

        private void UpdatePigStats() {
            if (!enclosureManager) return;

            Pig currentPig = enclosureManager.GetCurrentPig();                  // Display pig stats (if any selected)

            if (happinessSlider) {
                happinessSlider.value = currentPig?.Happiness ?? 0;
                happinessSlider.maxValue = currentPig?.HappinessMax ?? 100;
                if (_happinessText) _happinessText.text = $"{currentPig?.GetHappinessPercent() ?? 0}%";
            }
            if (hungerSlider) {
                hungerSlider.value = currentPig?.Hunger ?? 0;
                hungerSlider.maxValue = currentPig?.HungerMax ?? 100;
                if (_hungerText) _hungerText.text = $"{currentPig?.GetHungerPercent() ?? 0}%";
            }
            if (cleanlinessSlider) {
                cleanlinessSlider.value = currentPig?.Cleanliness ?? 0;
                cleanlinessSlider.maxValue = currentPig?.CleanlinessMax ?? 100;
                if (_cleanlinessText) _cleanlinessText.text = $"{currentPig?.GetCleanlinessPercent() ?? 0}%";
            }
            if (enduranceSlider) {
                enduranceSlider.value = currentPig?.Endurance ?? 0;
                enduranceSlider.maxValue = currentPig?.EnduranceMax ?? 100;
                if (_enduranceText) _enduranceText.text = $"{currentPig?.GetEndurancePercent() ?? 0}%";
            }
        }
    }
}
