using UnityEngine.UI;
using UnityEngine;
using Managers;
using TMPro;

namespace Pigs {
    public class PigWellBeingUI : MonoBehaviour {
        [Header("Data")]
        [SerializeField] private PigManager pigManager;
        //[SerializeField] private int pigIndex = 0;

        [Header("UI Bars")]
        [SerializeField] private Slider happinessBar;
        [SerializeField] private Slider satietyBar;
        [SerializeField] private Slider cleanPigBar;
        [SerializeField] private Slider cleanEnclosureBar;
    
        [Header("UI Texts")]
        [SerializeField] private TMP_Text happinessText;
        [SerializeField] private TMP_Text satietyText;
        [SerializeField] private TMP_Text cleanPigText;
        [SerializeField] private TMP_Text cleanEnclosureText;

        void Start() {
            if (!pigManager) pigManager = FindFirstObjectByType<PigManager>();
            happinessBar.maxValue =      100;
            satietyBar.maxValue =        100;
            cleanPigBar.maxValue =       100;
            cleanEnclosureBar.maxValue = 100;
        }

        void Update() {
            Pig pig = pigManager.GetCurrentPig();
            if (pig == null) return;

            happinessBar.value =      pig.Happiness;
            satietyBar.value =        pig.Hunger;
            cleanPigBar.value =       pig.Cleanliness;
            cleanEnclosureBar.value = 100;
        
            happinessText.text =      $"{pig.Happiness}%";
            satietyText.text =        $"{pig.Hunger}%";
            cleanPigText.text =       $"{pig.Cleanliness}%";
            cleanEnclosureText.text = $"{100}%";
        }
    }
}
