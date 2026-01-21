using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Enclosures;
using Breeding;
using Pigs;

// . Can open stats panel
// ! Make actions buttons works
// ! Update UIs based on selected pig
// L Change bg based on cleanliness
// L Animate actions buttons (from center to position)
// ! Make a presentation video
// ReSharper disable CollectionNeverQueried.Local
namespace Managers {
    public class PigManager : MonoBehaviour {                                   // Used in enclosure scene
        [Header("Enclosure Config")]
        [SerializeField] private int capacity = 10;
        [SerializeField] private Image bgImage;
        [SerializeField] private RectTransform enclosureArea;
        [SerializeField] private Text capacityText;
        [SerializeField] private Button cleanEnclosureButton;
        [SerializeField] private GameObject actionButtonsPrefab;
        [SerializeField] private GameObject pigVisualPrefab;
        [SerializeField] private List<Sprite> dirtyLevelImages;

        [Header("Stats UI")]
        [SerializeField] private PigWellBeingUI statsPanel;

        [Header("Starting Pigs (Debug)")]
        [SerializeField] private List<PigData> pigs = new();                    // Drag and drop pig prefabs in list

        private readonly List<Pig> _pigsInEnclosure = new();
        private readonly List<Button> _actionButtons = new();
        private UIPigActionPanel _actionPanel;
        private Pig _currentPig;
        private bool _isFull;
        
        public List<Pig> Pigs => _pigsInEnclosure;

        private void Start() {
            _isFull = _pigsInEnclosure.Count >= capacity;

            if (capacityText) capacityText.text = $"Pigs {_pigsInEnclosure.Count} / {capacity.ToString()}";
            if (capacityText) capacityText.color = _isFull ? Color.red : Color.white;

            if (actionButtonsPrefab) {
                foreach (Button button in actionButtonsPrefab.GetComponentsInChildren<Button>())
                    _actionButtons.Add(button);
                actionButtonsPrefab.SetActive(false);
            }

            foreach (var data in pigs) AddPigToEnclosure(data.ToPig());
        }

        void Update() {
            foreach (var pig in _pigsInEnclosure) {                             // ! On going feature
                pig.DecrementAll(Time.deltaTime);
                pig.Cheer(-Time.deltaTime);
            }
        }

        private void AddPigToEnclosure(Pig newPig) {
            if (_isFull) return;

            GameObject go = Instantiate(pigVisualPrefab, enclosureArea);
            go.GetComponent<UIPigVisual>().Setup(newPig, enclosureArea, this);

            _pigsInEnclosure.Add(newPig);
            _isFull = _pigsInEnclosure.Count >= capacity;
            if (capacityText) capacityText.text = $"{_pigsInEnclosure.Count} / {capacity.ToString()}";
            if (capacityText) capacityText.color = _isFull ? Color.red : Color.white;
        }

        public void OnPigClick(Pig pig, Vector2 mousePosition) {
            _currentPig = pig;

            if (!_actionPanel) {
                GameObject go = Instantiate(actionButtonsPrefab, transform);    // Create in scene if doesn't exist
                _actionPanel = go.GetComponent<UIPigActionPanel>();
            }

            _actionPanel.gameObject.SetActive(true);
            _actionPanel.Setup(pig);

            RectTransform panelRT = _actionPanel.GetComponent<RectTransform>(); // Place at mouse position
            Canvas canvas = GetComponentInParent<Canvas>();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, 
                mousePosition, canvas.worldCamera, out var localPoint);

            panelRT.anchoredPosition = localPoint;
        }

        public Pig GetCurrentPig() => _currentPig;

        public Pig GetPig(int index) {
            return index >= 0 && index < _pigsInEnclosure.Count ? _pigsInEnclosure[index] : null;
        }

        public List<Pig> GetRarePigs(PigRarity minRarity) {
            List<Pig> result = new List<Pig>();
            foreach (var pig in _pigsInEnclosure) if (pig.Rarity >= minRarity) result.Add(pig);
            return result;
        }
    }
}
