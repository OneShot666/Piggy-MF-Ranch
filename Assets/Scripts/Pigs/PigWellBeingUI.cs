using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PigWellBeingUI : MonoBehaviour
{
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
    
    [SerializeField] private PigListUI pigListUI;

    void Start()
    {
        happinessBar.maxValue = 100;
        satietyBar.maxValue = 100;
        cleanPigBar.maxValue = 100;
        cleanEnclosureBar.maxValue = 100;
    }

    void Update()
    {
        Pig pig = pigManager.CurrentPig;
        if (pig == null) return;

        WellBeing wb = pig.WellBeing;
        
        happinessBar.value = wb.Happiness;
        satietyBar.value = wb.Satiety;
        cleanPigBar.value = wb.CleanlinessPig;
        cleanEnclosureBar.value = wb.CleanlinessEnclosure;
        
        happinessText.text = $"{wb.Happiness}%";
        satietyText.text = $"{wb.Satiety}%";
        cleanPigText.text = $"{wb.CleanlinessPig}%";
        cleanEnclosureText.text = $"{wb.CleanlinessEnclosure}%";
    }
}