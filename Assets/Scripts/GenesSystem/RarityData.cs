using UnityEngine;

namespace GenesSystem {
    [CreateAssetMenu(fileName = "RarityData", menuName = "Scriptable Objects/RarityData")]
    public class RarityData : ScriptableObject {
        public float maxStat;
        public float absoluteLimit;
        public float weight;
    }
}
