using UnityEngine;

public class PlayerActions : MonoBehaviour {
    public void FeedPig(Pig pig, float amount) {
        pig.WellBeing.Feed(amount);
    }
    public void CleanPig(Pig pig, float amount) {
        pig.WellBeing.Clean(amount);
    }
    public void CheerPig(Pig pig, float amount) {
        pig.WellBeing.Cheer(amount);
    }
}
