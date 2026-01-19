using UnityEngine;

namespace GenesSystem.Colors {
    [CreateAssetMenu(fileName = "SpecialColorRecipeData", menuName = "Scriptable Objects/SpecialColorRecipeData")]
    public class PassiveColorRecipeData : ScriptableObject {
        public PigColor parentColorA;
        public PigColor parentColorB;
        public PigColor result;
    }
}
