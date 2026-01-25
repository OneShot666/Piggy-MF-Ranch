using UnityEngine;

namespace Pigs {
    /// <summary> ScriptableObject class to have an object for each type of pig (based on rarity) </summary>
    [CreateAssetMenu(fileName = "NewPigData", menuName = "Pigs/Pig")]
    public class PigData : ScriptableObject {
        public string pigName = "Pinky";
        public Sprite icon;
        public PigColor color = PigColor.Pink;
        public PigRarity rarity = PigRarity.Common;
        public float baseSpeed = 3;
        public float baseEndurance = 100f;
        public PigActivePower activePower = PigActivePower.None;
        public PigPassivePower passivePower = PigPassivePower.None;

        public Pig ToPig() => new Pig(this);                                    // Convert to Pig instance
    }
}
