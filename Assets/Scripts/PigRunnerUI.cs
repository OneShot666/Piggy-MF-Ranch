using TMPro;
using UnityEngine;

public class PigRunnerUI : MonoBehaviour
{
    [Header("TMP dans le Canvas World Space du prefab")]
    [SerializeField] private TMP_Text nameText;

    private static readonly Color32 PlayerNameColor = new Color32(255, 0, 92, 255);

    void Awake()
    {
        if (nameText == null)
            nameText = GetComponentInChildren<TMP_Text>(true);
    }

    public void SetName(string displayName, bool isPlayer)
    {
        if (nameText == null) return;

        nameText.text = displayName;
        nameText.color = isPlayer ? PlayerNameColor : Color.white;
    }
}
