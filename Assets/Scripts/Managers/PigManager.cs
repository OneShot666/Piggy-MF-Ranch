using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Enclosures;
using Breeding;
using TMPro;
using Pigs;

// . Add race scene
// L Animate actions buttons (from center to position)
// L Add animation + slider for capacity window
// L Update resources panel in stats canvas (money, food & energy) => need player & inventory
// L Apply energy cost to action
// L Add juice for actions buttons (speed reduce, small "dance" rotation, dirty filter on pig, "ZZZ" text + stop moving)

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

        [Header("Dirtiness settings")]
        [SerializeField] private Slider enclosureDirtSlider;
        [SerializeField] private float baseDirtRate = 0.5f;
        [SerializeField] private int cleanAmount = 5;

        [Header("Starting Pigs")]
        [SerializeField] private List<PigData> pigs = new();                    // Drag and drop pig prefabs in list

        private readonly Dictionary<Pig, RectTransform> _pigsVisuals = new();
        private readonly List<Pig> _pigsInEnclosure = new();
        private readonly List<Button> _actionButtons = new();
        private ActionPanelManager _actionPanel;
        private TextMeshProUGUI _enclosureText;
        private Pig _currentPig;
        private bool _isFull;
        private readonly int _maxDirty = 100;
        private float _currentDirtiness;

        public List<Pig> Pigs => _pigsInEnclosure;

        private void Start() {
            _enclosureText = enclosureDirtSlider.GetComponentInChildren<TextMeshProUGUI>();

            VerifyBaseSprite();
            UpdateCapacityUI();

            if (actionButtonsPrefab) {
                foreach (Button button in actionButtonsPrefab.GetComponentsInChildren<Button>())
                    _actionButtons.Add(button);
                actionButtonsPrefab.SetActive(false);                           // Hide action buttons panel by default
            }

            foreach (var data in pigs) AddPigToEnclosure(data.ToPig());         // Create pigs in enclosure
        }

        void Update() {
            UpdateDirtiness();
            foreach (var pig in _pigsInEnclosure) pig.DecrementAll(Time.deltaTime); // Pigs losing stats over time
        }

        private void AddPigToEnclosure(Pig newPig) {
            if (_isFull) return;

            GameObject go = Instantiate(pigVisualPrefab, enclosureArea);
            go.GetComponent<UIPigVisual>().Setup(newPig, enclosureArea, this);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-enclosureArea.rect.width / 2f, 
                enclosureArea.rect.width / 2f), Random.Range(-enclosureArea.rect.height / 2f, enclosureArea.rect.height / 2f));
            _pigsInEnclosure.Add(newPig);
            _pigsVisuals.Add(newPig, go.GetComponent<RectTransform>());
            
            UpdateCapacityUI();
        }

        private void UpdateCapacityUI() {
            _isFull = _pigsInEnclosure.Count >= capacity;
            if (capacityText) capacityText.text = $"{_pigsInEnclosure.Count}/{capacity.ToString()}";
            if (capacityText) capacityText.color = _isFull ? Color.red : Color.white;
        }

        public void OnPigClick(Pig pig, Vector2 mousePosition) {
            _currentPig = pig;

            if (!_actionPanel) {
                GameObject go = Instantiate(actionButtonsPrefab, transform);    // Create in scene if doesn't exist
                _actionPanel = go.GetComponent<ActionPanelManager>();
            }

            _actionPanel.gameObject.SetActive(true);
            if (_pigsVisuals.TryGetValue(pig, out RectTransform visualRT)) _actionPanel.Setup(_currentPig, visualRT);
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

        public void RemovePig(Pig pig, GameObject go) {
            _pigsInEnclosure.Remove(pig);
            Destroy(go);
            UpdateCapacityUI();
        }

        private void VerifyBaseSprite() {
            if (!bgImage || !bgImage.sprite) return;

            if (dirtyLevelImages.Count == 0 || dirtyLevelImages[0] != bgImage.sprite)
                dirtyLevelImages.Insert(0, bgImage.sprite);                     // Add default bg image if isn't
        }

        private void UpdateDirtiness() {
            if (_pigsInEnclosure.Count == 0) return;

            float dirtGain = baseDirtRate * _pigsInEnclosure.Count * Time.deltaTime;
            _currentDirtiness = Mathf.Min(_maxDirty, _currentDirtiness + dirtGain);   // Dirtiness increase by on number of pigs

            if (enclosureDirtSlider) {                                          // Update slider
                enclosureDirtSlider.value = _currentDirtiness;
                enclosureDirtSlider.maxValue = _maxDirty;
                _enclosureText.text = $"{Mathf.Round(_currentDirtiness / _maxDirty * 100)}%";
            }

            UpdateBackgroundVisual();
        }

        private void UpdateBackgroundVisual() {                                 // Update bg based on dirtiness
            if (dirtyLevelImages.Count == 0) return;

            float ratio = _currentDirtiness / _maxDirty;                         // Get index
            int imageIndex = Mathf.FloorToInt(ratio * dirtyLevelImages.Count);

            imageIndex = Mathf.Clamp(imageIndex, 0, dirtyLevelImages.Count - 1);    // Security

            // Update bg image
            if (bgImage.sprite != dirtyLevelImages[imageIndex]) bgImage.sprite = dirtyLevelImages[imageIndex];
        }

        public void CleanEnclosure() {                                          // Clean enclosure
            _currentDirtiness = Mathf.Max(0f, _currentDirtiness - cleanAmount);
            UpdateBackgroundVisual();
        }
    }
}
