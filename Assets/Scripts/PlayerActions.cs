using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PigManager pigManager;

	private Pig CurrentPig => pigManager.CurrentPig;

    [Header("Action values")]
    [SerializeField] private float cheerAmount = 1f;
    [SerializeField] private float feedAmount = 2f;
    [SerializeField] private float cleanPigAmount = 2f;
    [SerializeField] private float cleanEnclosureAmount = 2f;

    public void FeedPig()
    {
        if (CurrentPig == null) return;
        CurrentPig.WellBeing.Feed(feedAmount);
    }

    public void CleanPig()
    {
        if (CurrentPig == null) return;
        CurrentPig.WellBeing.CleanPig(cleanPigAmount);
    }

    public void CleanEnclosure()
    {
        if (CurrentPig == null) return;
        CurrentPig.WellBeing.CleanEnclosure(cleanEnclosureAmount);
    }

    public void CheerPig()
    {
        if (CurrentPig == null) return;
        CurrentPig.WellBeing.Cheer(cheerAmount);
    }
}