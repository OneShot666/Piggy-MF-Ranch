using UnityEngine;

namespace Pigs {
    [CreateAssetMenu(fileName = "NewPigData", menuName = "Pigs/Pig")]
    public class PigData : ScriptableObject {
        public Sprite icon;
        public PigColor color;
        public PigRarity rarity;
        public float baseSpeed;
        public float baseEndurance = 100f;
        public PigPassivePower passivePower;
        public PigActivePower activePower;

        public Pig ToPig() => new Pig(this);                                    // Convert to Pig instance
    }
}
