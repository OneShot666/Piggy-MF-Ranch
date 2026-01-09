using UnityEngine;
using UnityEngine.UI;

public class PigWellBeingUI : MonoBehaviour
{
    public PigWellBeing pig;
    public Text hungerText;
    public Text happinessText;
    public Text cleanlinessText;

    void Start()
    {
        pig.OnValuesChanged += RefreshUI;
        RefreshUI();
    }

    void RefreshUI()
    {
        hungerText.text = $"Faim : {Mathf.RoundToInt(pig.satiety)}";
        happinessText.text = $"Bonheur : {Mathf.RoundToInt(pig.happiness)}";
        cleanlinessText.text = $"Propreté : {Mathf.RoundToInt(pig.cleanlinessPig)}";
    }
}