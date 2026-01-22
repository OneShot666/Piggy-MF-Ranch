using UnityEngine;
using Breeding;

namespace Pigs {
    [System.Serializable]
    public class Pig {
        [Header("Identity")]
        public Sprite Icon { get; }
        public string name = "Pig";
        public float Health { get; set; } = 100f;
        public PigColor SkinColor { get; }
        public PigRarity Rarity { get; }
        public float Speed { get; set; }
        public int Generation { get; }
        public int MutationBonus { get; set; }

        [Header("Powers")]
        public PigPassivePower? PassivePower { get; }
        public PigActivePower? ActivePower { get; }

        [Header("Status")]
        public float HungerMax { get; set; } =      100f;
        public float HappinessMax { get; set; } =   100f;
        public float CleanlinessMax { get; set; } = 100f;
        public float EnduranceMax { get; set; } =   100f;

        private float _hunger =      100f;
        private float _happiness =   100f;
        private float _cleanliness = 100f;
        private float _endurance =   100f;

        public float HealthMax => 100f;
        public float Hunger => _hunger;
        public float Happiness => _happiness;
        public float Cleanliness => _cleanliness;
        public float Endurance => _endurance;

        #region Constructors
        public Pig(PigData data) {                                              // Use ScriptableObject data
            Icon = data.icon; SkinColor = data.color; Rarity = data.rarity;
            Speed = data.baseSpeed; EnduranceMax = data.baseEndurance;
            _endurance = data.baseEndurance; PassivePower = data.passivePower;
            ActivePower = data.activePower; Generation = 1;
        }

        public Pig(Sprite icon, PigColor skinColor, PigRarity rarity, float speed, PigPassivePower? passive, 
            PigActivePower? active, int mutation, int gen) {                    // Used in breeding features
            Icon = icon; SkinColor = skinColor; Rarity = rarity; Speed = speed;
            PassivePower = passive; ActivePower = active;
            MutationBonus = mutation; Generation = gen;
        }
        #endregion

        #region Getters
        public int GetHungerPercent() => Mathf.RoundToInt(_hunger / HungerMax * 100);
        public int GetHappinessPercent() => Mathf.RoundToInt(_happiness / HappinessMax * 100);
        public int GetCleanlinessPercent() => Mathf.RoundToInt(_cleanliness / CleanlinessMax * 100);
        public int GetEndurancePercent() => Mathf.RoundToInt(_endurance / EnduranceMax * 100);
        #endregion

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

        public void Feed(float amount) => _hunger = Mathf.Min(HungerMax, _hunger + amount);

        public void Cheer(float amount) => _happiness = Mathf.Min(HappinessMax, _happiness + amount);

        public void Clean(float amount) => _cleanliness = Mathf.Min(CleanlinessMax, _cleanliness + amount);

        public void Rest(float amount) => _endurance = Mathf.Min(EnduranceMax, _endurance + amount);

        public bool IsFitForBreeding() => _hunger >= 50f && _happiness >= 50f && _cleanliness >= 50f;

        public float GetGlobalWellBeing() => (_hunger + _happiness + _cleanliness) / 300f;

        public void FailedToGainPower() => MutationBonus = Mathf.Min(100, MutationBonus + 5);

        public Color GetColor() {
            return SkinColor switch {
                PigColor.Pink => Color.pink,
                PigColor.Brown => Color.saddleBrown,
                PigColor.Black => Color.black,
                PigColor.White => Color.white,
                PigColor.Golden => Color.goldenRod,
                PigColor.Rainbow => Color.red,                                  // ? Animate
                PigColor.Grey => Color.gray,
                PigColor.Beige => Color.softYellow,
                PigColor.DarkGold => new Color(0.6f, 0.4f, 0.0f),
                _ => Color.black
            };
        }
        #endregion
    }
}
