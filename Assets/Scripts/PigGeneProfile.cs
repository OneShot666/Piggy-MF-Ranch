using UnityEngine;

namespace GeneSystem
{
    [CreateAssetMenu(fileName = "PigGeneProfile", menuName = "Scriptable Objects/PigGeneProfile")]
    public class PigGeneProfile : ScriptableObject
    {
        public string colorName;
        public GeneRarityEnum rarity;
        public float speedBonus;
        public float enduranceBonus;
        public string specialPower;
    }
}
