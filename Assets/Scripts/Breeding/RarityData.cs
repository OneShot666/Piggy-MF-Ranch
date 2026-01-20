using UnityEngine;

namespace Breeding {
    [CreateAssetMenu(fileName = "RarityData", menuName = "Pigs/RarityData")]
    public class RarityData : ScriptableObject {
        public PigRarity rarity;
        public float maxStat;
        public float absoluteLimit;
        public float coeff;
    }
}
