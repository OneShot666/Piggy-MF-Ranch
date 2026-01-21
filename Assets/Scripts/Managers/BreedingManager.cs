using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.UI;
using UnityEngine;
using Breeding;
using Pigs;

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

        private void Start() {
            _isFull = _pigsInEnclosure.Count >= capacity;
            
            if (capacityText) capacityText.text = $"{_pigsInEnclosure.Count} / {capacity.ToString()}";
            if (capacityText) capacityText.color = _isFull ? Color.red : Color.black;
            if (parentSlot1) parentSlot1.onClick.AddListener(ResetParent1);
            if (parentSlot2) parentSlot2.onClick.AddListener(ResetParent2);
            if (slotImage1) slotImage1.gameObject.SetActive(false);
            if (slotImage2) slotImage2.gameObject.SetActive(false);
            if (breedButton) breedButton.gameObject.SetActive(false);
            if (breedButton) breedButton.onClick.AddListener(OpenConfirmationPopup);
            if (confirmationWindow) confirmationWindow.SetActive(false);

            foreach (var data in pigs) AddPigToEnclosure(data.ToPig());
        }

        private void AddPigToEnclosure(Pig newPig) {
            if (_isFull) return;

            GameObject go = Instantiate(pigVisualPrefab, enclosureArea);
            go.GetComponent<UIPigVisual>().Setup(newPig, enclosureArea, this);

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
            Pig baby = FindFirstObjectByType<BreedingSystem>().Breed(_parent1, _parent2);
            if (baby != null) AddPigToEnclosure(baby);                          // L Manage error

            confirmationWindow.SetActive(false);                                // Hide window
            ResetParent1(); ResetParent2();
        }
    }
}
