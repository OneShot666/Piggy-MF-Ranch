using UnityEngine;
using Save;

namespace Pigs {
    [System.Serializable]
    public class Pig {
        [Header("Identity")]
        public Sprite Icon { get; }
        public string pigName = "Pinky";
        public float Health { get; set; } = 100f;
        public PigColor SkinColor { get; }
        public PigRarity Rarity { get; set; }
        public float Speed { get; set; }
        public int Generation { get; set;  }
        public int MutationBonus { get; set; }

        [Header("Powers")]
        public PigActivePower? ActivePower { get; set; }
        public PigPassivePower? PassivePower { get; set; }

        [Header("Status")]
        public int HungerMax { get; set; } =      100;
        public int HappinessMax { get; set; } =   100;
        public int CleanlinessMax { get; set; } = 100;
        public int EnduranceMax { get; set; } =   100;

        public string BaseDataName { get; set; }

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
        public Pig(Sprite icon=null) {                                          // Default pig
            Icon = icon; SkinColor = PigColor.Pink; Rarity = PigRarity.Common;
            Speed = 3; EnduranceMax = 100; _endurance = 100; Generation = 1;
            ActivePower = PigActivePower.None; PassivePower = PigPassivePower.None;
        }

        public Pig(PigData data) {                                              // Use ScriptableObject data
            pigName = data.pigName; BaseDataName = data.pigName; Icon = data.icon; 
            SkinColor = data.color; Rarity = data.rarity;
            Speed = data.baseSpeed; EnduranceMax = (int)data.baseEndurance;
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

        #region Save functions
        /// <summary> Restore pig intern state from save</summary>
        public void LoadSaveData(PigSaveData data) {
            pigName = data.pigName;
            Health = data.health;
            if (System.Enum.TryParse(data.rarity, out PigRarity loadedRarity)) Rarity = loadedRarity;
            Speed = data.speed;
            Generation = data.generation;
            MutationBonus = data.mutationBonus;
            if (System.Enum.TryParse(data.activePower, out PigActivePower loadedActive)) ActivePower = loadedActive;
            if (System.Enum.TryParse(data.passivePower, out PigPassivePower loadedPassive)) PassivePower = loadedPassive;
            _hunger = data.hunger;
            HungerMax = data.hungerMax;
            _happiness = data.happiness;
            HappinessMax = data.happinessMax;
            _cleanliness = data.clean;
            CleanlinessMax = data.cleanMax;
            _endurance = data.endurance;
            EnduranceMax = data.enduranceMax;
        }

        public PigSaveData GetSaveData() => new() {
            pigName = pigName, health = Health, speed = Speed,
            color = (int)SkinColor, rarity = Rarity.ToString(),
            generation = Generation, mutationBonus = MutationBonus,
            activePower = ActivePower.ToString(), passivePower = PassivePower.ToString(),
            hunger = _hunger, hungerMax = HungerMax,
            happiness = _happiness, happinessMax = HappinessMax,
            clean = _cleanliness, cleanMax = CleanlinessMax,
            endurance = _endurance, enduranceMax = EnduranceMax
        };
        #endregion
    }
}
