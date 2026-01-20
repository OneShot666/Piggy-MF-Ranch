using Pigs;
using UnityEngine;

namespace Basic {
    public class PlayerActions : MonoBehaviour {
        public void FeedPig(Pig pig, float amount) {
            pig.Feed(amount);
        }
        public void CleanPig(Pig pig, float amount) {
            pig.Clean(amount);
        }
        public void CheerPig(Pig pig, float amount) {
            pig.Cheer(amount);
        }
    }
}
