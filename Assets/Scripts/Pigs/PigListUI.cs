using UnityEngine;

public class PigListUI : MonoBehaviour
{
    [SerializeField] private PigManager pigManager;
    [SerializeField] private Transform content;
    [SerializeField] private PigButtonUI pigButtonPrefab;

    public Pig SelectedPig { get; private set; }

    void Start()
    {
        RefreshList();
    }

    void RefreshList()
    {
        if (content == null)
        {
            Debug.LogError("Content n'est pas assigné !");
            return;
        }

        // Sécurité : ne détruire que des objets de scène
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        foreach (var pig in pigManager.pigs)
        {
            PigButtonUI button = Instantiate(pigButtonPrefab, content, false);
            button.Init(pig, OnPigSelected);
        }

        if (pigManager.pigs.Count > 0)
            OnPigSelected(pigManager.pigs[0]);
    }

    void OnPigSelected(Pig pig)
    {
        SelectedPig = pig;
        Debug.Log($"Cochon sélectionné : {pig.Color}");
    }
}