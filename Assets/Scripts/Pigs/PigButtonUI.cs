using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PigButtonUI : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private Button button;
    [SerializeField] private PigManager pigManager;
    private Pig pig;

    public void Init(Pig pig)
    {
        this.pig = pig;
        label.text = $"{pig.Color} ({pig.Rarity})";
    }

    public void OnClick()
    {
        pigManager.SelectPig(pig);
        Debug.Log("Pig sélectionné : " + pig.Color);
    }
}