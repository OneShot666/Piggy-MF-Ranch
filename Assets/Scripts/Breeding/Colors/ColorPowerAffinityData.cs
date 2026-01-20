using UnityEngine;

namespace Breeding.Colors {
    [CreateAssetMenu(fileName = "ColorPowerAffinityData", menuName = "Pigs/Color Power")]
    public class ColorPowerAffinityData : ScriptableObject {
        public PigColor color;
        public PigPassivePower favoredPower;
        public float powerBoostMultiplier = 5f;
    }
}
