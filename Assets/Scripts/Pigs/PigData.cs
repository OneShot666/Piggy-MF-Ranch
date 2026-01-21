using UnityEngine;

namespace Pigs {
    /// <summary> ScriptableObject class to have an object for each type of pig (based on rarity) </summary>
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
