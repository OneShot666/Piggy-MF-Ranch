using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Pigs;

// . Displaying pigs in breed enclosure
// ! Add overlay when mouse hover a pig
// ! Selection panel don't update for parent image
// ! Update full confirmation window + set texts
// ! New born don't have icon
namespace Breeding {
    public class BreedingManager : MonoBehaviour {
        [Header("Enclosure Config")]
        public int capacity = 10;
        public RectTransform enclosureArea;
        public GameObject pigVisualPrefab;

        [Header("Breeding UI")]
        public Button breedButton;
        public GameObject confirmationWindow;                                   // Manage confirmation window

        [Header("Starting Pigs (Debug)")]
        public List<PigData> pigs = new();                                      // Drag and drop pig prefabs in list

        private readonly List<Pig> _pigsInEnclosure = new();
        private Pig _parent1;
        private Pig _parent2;

        private void Start() {
            breedButton.gameObject.SetActive(false);
            confirmationWindow.SetActive(false);
            breedButton.onClick.AddListener(OpenConfirmationPopup);

            foreach (var data in pigs) AddPigToEnclosure(data.ToPig());
        }

        private void AddPigToEnclosure(Pig newPig) {
            if (_pigsInEnclosure.Count >= capacity) return;                     // Enclosure full

            _pigsInEnclosure.Add(newPig);
            GameObject go = Instantiate(pigVisualPrefab, enclosureArea);
            go.GetComponent<UIPigVisual>().Setup(newPig, enclosureArea, this);
        }

        public void SelectParent(Pig pig) {
            if (_parent1 == pig) { _parent1 = null; }
            else if (_parent2 == pig) { _parent2 = null; }
            else if (_parent1 == null) { _parent1 = pig; }
            else if (_parent2 == null) { _parent2 = pig; }
            else { _parent1 = pig; _parent2 = null; }

            breedButton.gameObject.SetActive(_parent1 != null && _parent2 != null); // Display breed button when both parent
        }

        private void OpenConfirmationPopup() {
            confirmationWindow.SetActive(true);
            UIBreedingConfirmation script = confirmationWindow.GetComponent<UIBreedingConfirmation>();
            script.SetupPopup(_parent1, _parent2, ConfirmBreeding);
        }

        private void ConfirmBreeding() {                    // Called when player confirm breeding on informations window
            Pig baby = FindFirstObjectByType<BreedingSystem>().Breed(_parent1, _parent2);
            if (baby != null) AddPigToEnclosure(baby);                          // L Manage error

            confirmationWindow.SetActive(false);
            _parent1 = null; _parent2 = null;
            breedButton.gameObject.SetActive(false);
        }
    }
}
