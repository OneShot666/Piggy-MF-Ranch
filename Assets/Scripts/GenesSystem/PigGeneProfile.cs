using UnityEngine;

namespace GenesSystem {
    [CreateAssetMenu(fileName = "PigGeneProfile", menuName = "Scriptable Objects/PigGeneProfile")]
    public class PigGeneProfile : ScriptableObject {
        public PigColor color;
        public PigRarity rarity;
        public float speedBonus;
        public float enduranceBonus;
        public float luckBonus;
        public float xpBonus;
        public PigPassivePower passivePower;
        public PigActivePower activePower;
    }
}
