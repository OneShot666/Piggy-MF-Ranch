using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using Managers;
using Pigs;

namespace Breeding {
    /// <summary> Class to visualize pigs in breed enclosure. </summary>
    public class UIPigVisual : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
        [Header("Settings")]
        [SerializeField] private float moveSpeed = 50f;
        [SerializeField] private float changeTargetTime = 3f;
        [SerializeField] private Image pigRenderer;
        [SerializeField] private Image selectedRenderer;

        private Pig _data;
        private PigManager _pigManager;
        private BreedingManager _breedManager;
        private RectTransform _rectTransform;
        private RectTransform _container;
        private Vector2 _targetPosition;
        private float _timer;

        public void Setup(Pig data, RectTransform container, BreedingManager manager) {
            _data = data;
            _container = container;
            _breedManager = manager;
            _rectTransform = GetComponent<RectTransform>();

            if (pigRenderer) pigRenderer.sprite = data.Icon;
            if (selectedRenderer) selectedRenderer.enabled = false;
            SetNewDestination();
        }

        public void Setup(Pig data, RectTransform container, PigManager manager) {
            _data = data;
            _container = container;
            _pigManager = manager;
            _rectTransform = GetComponent<RectTransform>();

            if (pigRenderer) pigRenderer.sprite = data.Icon;
            if (selectedRenderer) selectedRenderer.enabled = false;
            SetNewDestination();
        }

        private void SetNewDestination() {                                      // Go to random position on container
            float halfW = _container.rect.width / 2f;
            float halfH = _container.rect.height / 2f;
            _targetPosition = new Vector2(Random.Range(-halfW, halfW), Random.Range(-halfH, halfH));
        }

        private void Update() {
            if (!_container) return;

            _rectTransform.anchoredPosition = Vector2.MoveTowards(_rectTransform.anchoredPosition, 
                _targetPosition, moveSpeed * Time.deltaTime);                   // Move to target position

            UpdateFacingDirection();

            _timer += Time.deltaTime;
            if (_timer >= changeTargetTime || Vector2.Distance(_rectTransform.anchoredPosition, _targetPosition) < 1f) {
                SetNewDestination();
                _timer = 0;
            }
        }

        private void UpdateFacingDirection() {
            float deltaX = _targetPosition.x - _rectTransform.anchoredPosition.x;   // Calculate next direction

            if (Mathf.Abs(deltaX) > 0.1f) {                                     // If pig moving
                float scaleX = deltaX > 0 ? -1f : 1f;                           // Change orientation if change direction

                if (pigRenderer) pigRenderer.transform.localScale = new Vector3(scaleX, 1f, 1f);    // Apply scale to image
            }
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (selectedRenderer) selectedRenderer.enabled = true;
            if (_data != null) UIPigOverlayManager.Instance?.Show(_data);
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (selectedRenderer) selectedRenderer.enabled = false;
            UIPigOverlayManager.Instance?.Hide();
        }

        public void OnPointerClick(PointerEventData eventData) {                // When click on pig
            if (_breedManager) _breedManager.SelectParent(_data);
            else if (_pigManager) _pigManager.OnPigClick(_data, eventData.position);
        }

        private void OnDisable() {
            if (selectedRenderer) selectedRenderer.enabled = false;
            UIPigOverlayManager.Instance?.Hide();
        }
    }
}
