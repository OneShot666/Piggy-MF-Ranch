using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using Managers;
using Races;
using Pigs;

namespace Breeding {
    /// <summary> Class to visualize pigs in breed enclosure. </summary>
    public class UIPigVisual : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
        [Header("Settings")]
        [SerializeField] private float moveSpeed = 50f;
        [SerializeField] private Vector2 changeTargetTimeRange = new(2, 8);
        [SerializeField] private Image selectedImage;
        [SerializeField] private Image pigImage;
        [SerializeField] private Text nameText;

        [Header("Health UI")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private float healPerSecond = 1f; 
        [SerializeField] private float damagePerSecond = 3f; 

        private Pig _data;
        private EnclosureManager _enclosureManager;
        private BreedingManager _breedManager;
        private RaceManager _raceManager;
        private RectTransform _rectTransform;
        private RectTransform _container;
        private Vector2 _targetPosition;
        private static readonly Color PlayerNameColor = new(255, 0, 92, 255);
        private float _changeTimer;
        private float _timer;
        private bool _isRacing;
        
        public Pig Data => _data;

        private void CommonSetUp(Pig data, RectTransform container) {
            _rectTransform = GetComponent<RectTransform>();

            _data = data;
            _container = container;

            InitVisuals();
        }

        public void Setup(Pig data, RectTransform container, BreedingManager manager) {
            _breedManager = manager;
            CommonSetUp(data, container);
        }

        public void Setup(Pig data, RectTransform container, EnclosureManager manager) {
            _enclosureManager = manager;
            CommonSetUp(data, container);
        }

        public void Setup(Pig data, RectTransform container, RaceManager manager) {
            _raceManager = manager;
            CommonSetUp(data, container);
        }

        public void SetRunnerMode(string displayName, bool isPlayer) {
            _isRacing = true;                                                   // Disable random movements (wandering)

            if (nameText) {
                nameText.enabled = true;
                nameText.text = displayName;
                nameText.color = isPlayer ? PlayerNameColor : Color.white;
            }
        }

        private void InitVisuals() {
            if (pigImage) pigImage.sprite = _data.Icon;
            if (selectedImage) selectedImage.enabled = false;
            if (nameText) nameText.enabled = false;

            if (healthSlider) {                                                 // Init life slider
                healthSlider.value = _data.Health;
                healthSlider.maxValue = _data.HealthMax;
                healthSlider.gameObject.SetActive(_data.Health < 100f);         // Hide by default
            }
            
            SetNewDestination();
        }

        private void SetNewDestination() {                                      // Go to random position on container
            _changeTimer = Random.Range(changeTargetTimeRange.x, changeTargetTimeRange.y);
            float halfW = _container.rect.width / 2f;
            float halfH = _container.rect.height / 2f;
            _targetPosition = new Vector2(Random.Range(-halfW, halfW), Random.Range(-halfH, halfH));
        }

        private void Update() {
            if (!_container || _isRacing) return;

            _rectTransform.anchoredPosition = Vector2.MoveTowards(_rectTransform.anchoredPosition, 
                _targetPosition, moveSpeed * Time.deltaTime);                   // Move to target position

            UpdateFacingDirection();

            HandleHealthLogic();

            _timer += Time.deltaTime;
            if (_timer >= _changeTimer || Vector2.Distance(_rectTransform.anchoredPosition, _targetPosition) < 1f) {
                SetNewDestination();
                _timer = 0;
            }
        }

        private void UpdateFacingDirection() {
            float deltaX = _targetPosition.x - _rectTransform.anchoredPosition.x;   // Calculate next direction

            if (Mathf.Abs(deltaX) > 0.1f) {                                     // If pig moving
                float scaleX = deltaX > 0 ? -1f : 1f;                           // Change orientation if change direction

                if (pigImage) pigImage.transform.localScale = new Vector3(scaleX, 1, 1);  // Apply orientation to image
            }
        }

        public void UpdateFacingDirection(bool facingRight) {
            float scaleX = facingRight ? -1f : 1f;
            if (pigImage) pigImage.transform.localScale = new Vector3(scaleX, 1, 1);
        }

        private void HandleHealthLogic() {
            // Check if has a stat to 0
            bool isInDanger = _data.Hunger <= 0 || _data.Happiness <= 0 || _data.Cleanliness <= 0 || _data.Endurance <= 0;

            if (isInDanger)
                _data.Health = Mathf.Max(0, _data.Health - damagePerSecond * Time.deltaTime);   // Piggy slowly dies
            else if (_data.Health < _data.HealthMax)
                _data.Health = Mathf.Min(_data.HealthMax, _data.Health + healPerSecond * Time.deltaTime);

            if (healthSlider) {                                                 // Update life slider
                healthSlider.value = _data.Health;
                
                bool showBar = _data.Health is < 100f and > 0;                  // Show if hurt
                if (healthSlider.gameObject.activeSelf != showBar) healthSlider.gameObject.SetActive(showBar);
            }

            if (_data.Health <= 0) _enclosureManager.RemovePig(_data, gameObject);    // Piggy dies
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (selectedImage) selectedImage.enabled = true;
            if (_data != null) UIPigOverlayManager.Instance?.Show(_data);
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (selectedImage) selectedImage.enabled = false;
            UIPigOverlayManager.Instance?.Hide();
        }

        public void OnPointerClick(PointerEventData eventData) {                // When click on pig
            if (_breedManager) _breedManager.SelectParent(_data);
            else if (_enclosureManager) _enclosureManager.OnPigClick(_data, eventData.position);
        }

        private void OnDisable() {
            if (selectedImage) selectedImage.enabled = false;
            UIPigOverlayManager.Instance?.Hide();
        }
    }
}
