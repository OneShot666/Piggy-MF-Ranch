using Random = UnityEngine.Random;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.UI;
using UnityEngine;
using Breeding;
using Pigs;
using Save;

// L Add timer to breed new piglet
// ? Add growth to pigs (maturity)
namespace Managers {
    public class BreedingManager : MonoBehaviour {
        [Header("Enclosure Config")]
        [SerializeField] private int capacity = 10;
        [SerializeField] private RectTransform enclosureArea;
        [SerializeField] private Text capacityText;
        [SerializeField] private Button parentSlot1;
        [SerializeField] private Button parentSlot2;
        [SerializeField] private Image slotImage1;
        [SerializeField] private Image slotImage2;
        [SerializeField] private GameObject pigVisualPrefab;

        [Header("Breeding UI")]
        [SerializeField] private Button breedButton;
        [SerializeField] private GameObject confirmationWindow;                 // Manage confirmation window

        [Header("Starting Pigs (Debug)")]
        [SerializeField] private List<PigData> pigs = new();                    // Drag and drop pig prefabs in list

        private readonly List<Pig> _pigsInEnclosure = new();
        private Pig _parent1;
        private Pig _parent2;
        private bool _isFull;
        private bool _isInitialized;

        private void Start() {
            UpdateObjectInScene();

            if (GameManager.Instance && GameManager.Instance.currentSave.herd.Count > 0)
                LoadPigs(GameManager.Instance.currentSave.herd);
            else foreach (var data in pigs) AddPigToEnclosure(data.ToPig());

            _isInitialized = true;
        }

        private void UpdateObjectInScene() {
            _isFull = _pigsInEnclosure.Count >= capacity;                       // Check if still have room

            if (capacityText) capacityText.text = $"{_pigsInEnclosure.Count} / {capacity.ToString()}";
            if (capacityText) capacityText.color = _isFull ? Color.red : Color.black;
            if (parentSlot1) parentSlot1.onClick.AddListener(ResetParent1);
            if (parentSlot2) parentSlot2.onClick.AddListener(ResetParent2);
            if (slotImage1) slotImage1.gameObject.SetActive(false);             // Unselect parents
            if (slotImage2) slotImage2.gameObject.SetActive(false);
            if (breedButton) breedButton.gameObject.SetActive(false);
            if (breedButton) breedButton.onClick.AddListener(OpenConfirmationPopup);
            if (confirmationWindow) confirmationWindow.SetActive(false);
        }

        private void AddPigToEnclosure(Pig newPig) {
            if (_isFull) return;

            GameObject go = Instantiate(pigVisualPrefab, enclosureArea);
            go.GetComponent<UIPigVisual>().Setup(newPig, enclosureArea, this);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-enclosureArea.rect.width / 2f, 
                enclosureArea.rect.width / 2f), Random.Range(-enclosureArea.rect.height / 2f, enclosureArea.rect.height / 2f));

            _pigsInEnclosure.Add(newPig);
            _isFull = _pigsInEnclosure.Count >= capacity;
            if (capacityText) capacityText.text = $"{_pigsInEnclosure.Count} / {capacity.ToString()}";
            if (capacityText) capacityText.color = _isFull ? Color.red : Color.black;
        }

        private void SetParent1([CanBeNull] Pig newPig) {
            _parent1 = newPig;
            if (slotImage1) {
                slotImage1.sprite = newPig?.Icon;
                slotImage1.gameObject.SetActive(newPig != null);
            }
        }

        private void SetParent2([CanBeNull] Pig newPig) {
            _parent2 = newPig;
            if (slotImage2) {
                slotImage2.sprite = newPig?.Icon;
                slotImage2.gameObject.SetActive(newPig != null);
            }
        }

        private void ResetParent1() {
            SetParent1(null);
            breedButton.gameObject.SetActive(false);
        }

        private void ResetParent2() {
            SetParent2(null);
            breedButton.gameObject.SetActive(false);
        }

        public void SelectParent(Pig pig) {
            if (_parent1 == pig) SetParent1(null);
            else if (_parent2 == pig) SetParent2(null);
            else if (_parent1 == null) SetParent1(pig);
            else if (_parent2 == null) SetParent2(pig);
            else { SetParent1(pig); SetParent2(null); }

            breedButton.gameObject.SetActive(_parent1 != null && _parent2 != null); // Display breed button when both parent
        }

        private void OpenConfirmationPopup() {
            UIConfirmationBreeding script = confirmationWindow.GetComponent<UIConfirmationBreeding>();
            if (script) {
                script.OpenWindow();
                script.SetupPopup(_parent1, _parent2, ConfirmBreeding, _isFull);
            }
        }

        private void ConfirmBreeding() {                    // Called when player confirm breeding on informations window
            BreedingSystem system = FindFirstObjectByType<BreedingSystem>();
            Pig baby = system.Breed(_parent1, _parent2);
            if (baby != null) {                                                 // Add piglet to enclosure
                baby.BaseDataName = system.GetPigDataNameForColor(baby.SkinColor);
                AddPigToEnclosure(baby);
            }

            ResetParent1(); ResetParent2();
            confirmationWindow.SetActive(false);                                // Hide window
        }

        private void LoadPigs(List<PigSaveData> savedHerd) {
            if (savedHerd == null) return;

            foreach (var pSave in savedHerd) {
                PigData baseData = GameManager.Instance.allPossiblePigs.
                    Find(d => d.pigName == pSave.pigName);

                if (baseData) {
                    Pig loadedPig = baseData.ToPig();
                    loadedPig.BaseDataName = pSave.pigName;                     // Save's name is pig's name
                    loadedPig.LoadSaveData(pSave);
                    AddPigToEnclosure(loadedPig);
                }
            }
        }

        private void OnDisable() {
            if (!_isInitialized || !GameManager.Instance) return;

            List<PigSaveData> dataToSave = new List<PigSaveData>();
            foreach(var pig in _pigsInEnclosure) dataToSave.Add(pig.GetSaveData());
            GameManager.Instance.SyncPigs(dataToSave);
        }
    }
}
