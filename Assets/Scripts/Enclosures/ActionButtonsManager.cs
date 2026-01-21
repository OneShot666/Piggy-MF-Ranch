using UnityEngine.UI;
using UnityEngine;
using Pigs;

namespace Enclosures {
    public class UIPigActionPanel : MonoBehaviour {
        [SerializeField] private Button cheerButton;
        [SerializeField] private Button feedButton;
        [SerializeField] private Button cleanButton;
        [SerializeField] private Button encourageButton;

        private Pig _targetPig;
        private readonly float _bonus = 5;

        public void Setup(Pig pig) {
            _targetPig = pig;
            if (cheerButton) cheerButton.onClick.AddListener(() => pig.Cheer(_bonus));
            if (feedButton) feedButton.onClick.AddListener(() => pig.Feed(_bonus));
            if (cleanButton) cleanButton.onClick.AddListener(() => pig.Clean(_bonus));
            if (encourageButton) encourageButton.onClick.AddListener(() => pig.Rest(_bonus));
        }

        public void Close() {
            gameObject.SetActive(false);
        }
    }
}
