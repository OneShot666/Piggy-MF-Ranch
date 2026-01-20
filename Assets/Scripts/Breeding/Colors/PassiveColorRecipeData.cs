using UnityEngine;

namespace Breeding.Colors {
    [CreateAssetMenu(fileName = "PassiveColorRecipeData", menuName = "Pigs/Passive Color Recipe")]
    public class PassiveColorRecipeData : ScriptableObject {
        public PigColor parentColorA;
        public PigColor parentColorB;
        public PigColor result;
    }
}
