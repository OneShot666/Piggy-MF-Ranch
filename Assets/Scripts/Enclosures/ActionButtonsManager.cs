using UnityEngine.UI;
using UnityEngine;
using Pigs;

namespace Enclosures {
    public class ActionPanelManager : MonoBehaviour {
        [SerializeField] private Button cheerButton;
        [SerializeField] private Button feedButton;
        [SerializeField] private Button cleanButton;
        [SerializeField] private Button restButton;
        [SerializeField] private float bonus = 5;

        private RectTransform _rectTransform;
        private RectTransform _targetVisual;
        private Pig _targetPig;

        private void Awake() {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void Setup(Pig pig, RectTransform targetVisual) {                // Attributes action to buttons
            _targetPig = pig;
            _targetVisual = targetVisual;

            if (cheerButton) cheerButton.onClick.AddListener(() => _targetPig.Cheer(bonus));
            if (feedButton) feedButton.onClick.AddListener(() => _targetPig.Feed(bonus));
            if (cleanButton) cleanButton.onClick.AddListener(() => _targetPig.Clean(bonus));
            if (restButton) restButton.onClick.AddListener(() => _targetPig.Rest(bonus));

            UpdatePosition();
        }

        private void Update() {
            if (_targetVisual) UpdatePosition();
        }

        private void UpdatePosition() {                                         // Follow pig in scene
            _rectTransform.position = _targetVisual.position;
        }

        public void Close() {
            gameObject.SetActive(false);
        }
    }
}
