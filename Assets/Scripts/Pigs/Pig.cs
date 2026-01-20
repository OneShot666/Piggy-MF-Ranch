using UnityEngine;

namespace Pigs {
    [System.Serializable]
    public class Pig {
        [Header("Identity")]
        public Sprite Icon { get; }
        public PigColor Color { get; }
        public PigRarity Rarity { get; }
        public float Speed { get; set; }
        public int Generation { get; }
        public int MutationBonus { get; set; }

        [Header("Powers")]
        public PigPassivePower? PassivePower { get; }
        public PigActivePower? ActivePower { get; }

        [Header("Status")]
        private float _hunger = 100f;
        private float _happiness = 100f;
        private float _cleanliness = 100f;
        private float _endurance = 100f;
        public float HungerMax { get; set; } = 100f;
        public float HappinessMax { get; set; } = 100f;
        public float CleanlinessMax { get; set; } = 100f;
        public float EnduranceMax { get; set; } = 100f;

        public float Hunger => _hunger;
        public float Happiness => _happiness;
        public float Cleanliness => _cleanliness;
        public float Endurance => _endurance;

        // Constructeur à partir du ScriptableObject
        public Pig(PigData data) {
            Icon = data.icon;
            Color = data.color;
            Rarity = data.rarity;
            Speed = data.baseSpeed;
            EnduranceMax = data.baseEndurance;
            _endurance = data.baseEndurance;
            PassivePower = data.passivePower;
            ActivePower = data.activePower;
            Generation = 1;
        }

        // Constructeur pour la reproduction
        public Pig(PigColor color, PigRarity rarity, float speed, PigPassivePower? passive, 
            PigActivePower? active, int mutation, int gen, Sprite icon=null) {
            Icon = icon;
            Color = color;
            Rarity = rarity;
            Speed = speed;
            PassivePower = passive;
            ActivePower = active;
            MutationBonus = mutation;
            Generation = gen;
        }

        #region Condition Methods
        public bool InitBestCondition(bool changeHunger=true, bool changeHappiness=true, 
            bool changeCleanliness=true, bool changeEndurance=true) {
            bool changed = false;

            if (changeHunger && _hunger < HungerMax) { _hunger = HungerMax; changed = true; }
            if (changeHappiness && _happiness < HappinessMax) { _happiness = HappinessMax; changed = true; }
            if (changeCleanliness && _cleanliness < CleanlinessMax) { _cleanliness = CleanlinessMax; changed = true; }
            if  (changeEndurance && _endurance < EnduranceMax) { _endurance = EnduranceMax; changed = true; }

            return changed;
        }

        public void DecrementAll(float amount) {
            _hunger =      Mathf.Max(0, _hunger - amount);
            _happiness =   Mathf.Max(0, _happiness - amount);
            _cleanliness = Mathf.Max(0, _cleanliness - amount);
            _endurance =   Mathf.Max(0, _endurance - amount);
        }


        public void Feed(float amount) {
            _hunger = Mathf.Min(HungerMax, _hunger + amount);
        }

        public void Cheer(float amount) {
            _happiness = Mathf.Min(HappinessMax, _happiness + amount);
        }

        public void Clean(float amount) {
            _cleanliness = Mathf.Min(Cleanliness, _cleanliness + amount);
        }

        public void Rest(float amount) {
            _endurance = Mathf.Min(EnduranceMax, _endurance + amount);
        }

        public bool IsFitForBreeding() => _hunger >= 50f && _happiness >= 50f && _cleanliness >= 50f;

        public float GetGlobalWellBeing() => (_hunger + _happiness + _cleanliness) / 300f;

        public void FailedToGainPower() => MutationBonus += 5;

        // Retourne la couleur Unity correspondante au cochon (pour l'UI)
        public Color GetColor() {
            return Color switch {
                PigColor.Pink => new Color(1f, 0.75f, 0.8f), // Rose clair
                PigColor.Brown => new Color(0.54f, 0.27f, 0.07f), // SaddleBrown
                PigColor.Black => UnityEngine.Color.black,
                PigColor.White => UnityEngine.Color.white,
                PigColor.Golden => new Color(0.85f, 0.65f, 0.12f), // GoldenRod
                PigColor.Rainbow => UnityEngine.Color.red, // À animer plus tard ?
                PigColor.Grey => UnityEngine.Color.gray,
                PigColor.Beige => new Color(0.96f, 0.96f, 0.86f),
                PigColor.DarkGold => new Color(0.6f, 0.4f, 0.0f),
                _ => UnityEngine.Color.magenta
            };
        }
        #endregion
    }
}
