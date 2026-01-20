using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using Pigs;

namespace Breeding {
    public class UIPigVisual : MonoBehaviour, IPointerClickHandler {
        [Header("Settings")]
        [SerializeField] private float moveSpeed = 50f;
        [SerializeField] private float changeTargetTime = 3f;
        [SerializeField] private Image pigRenderer;

        private Pig _data;
        private RectTransform _rectTransform;
        private RectTransform _container;
        private Vector2 _targetPosition;
        private float _timer;
        private BreedingManager _manager;

        public void Setup(Pig data, RectTransform container, BreedingManager manager) {
            _data = data;
            _container = container;
            _manager = manager;
            _rectTransform = GetComponent<RectTransform>();

            if (pigRenderer) pigRenderer.sprite = data.Icon;
            SetNewTarget();
        }

        private void Update() {
            if (!_container) return;

            _rectTransform.anchoredPosition = Vector2.MoveTowards(_rectTransform.anchoredPosition, _targetPosition, moveSpeed * Time.deltaTime);

            _timer += Time.deltaTime;
            if (_timer >= changeTargetTime || Vector2.Distance(_rectTransform.anchoredPosition, _targetPosition) < 1f) {
                SetNewTarget();
                _timer = 0;
            }
        }

        private void SetNewTarget() {
            float halfW = _container.rect.width / 2f;
            float halfH = _container.rect.height / 2f;
            _targetPosition = new Vector2(Random.Range(-halfW, halfW), Random.Range(-halfH, halfH));
        }

        public void OnPointerClick(PointerEventData eventData) {
            _manager.SelectParent(_data);
        }
    }
}
