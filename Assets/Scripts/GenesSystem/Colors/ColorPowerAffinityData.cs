using UnityEngine;

namespace GenesSystem.Colors {
    [CreateAssetMenu(fileName = "ColorPowerAffinityData", menuName = "Scriptable Objects/ColorPowerAffinityData")]
    public class ColorPowerAffinityData : ScriptableObject {
        public PigColor color;
        public PigPassivePower favoredPower;
        public float powerBoostMultiplier = 5f;
    }
}
