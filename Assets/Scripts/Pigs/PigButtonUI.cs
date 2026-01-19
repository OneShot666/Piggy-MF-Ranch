using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PigButtonUI : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private Button button;
    private Pig pig;
    private Action<Pig> onSelected;

    public void Init(Pig pig, Action<Pig> onSelected)
    {
        this.pig = pig;
        this.onSelected = onSelected;

        label.text = $"{pig.Color} ({pig.Rarity})";
    }

    public void OnClick()
    {
        onSelected?.Invoke(pig);
    }
}