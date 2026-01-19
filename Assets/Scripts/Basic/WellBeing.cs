using UnityEngine;

// ? Merge in Pig class
// ReSharper disable MemberCanBePrivate.Global
namespace Basic {
    public class WellBeing {
        protected float hunger = 100f;
        protected readonly float hungerMax = 100f;
        protected float happiness = 100f;
        protected readonly float happinessMax = 100f;
        protected float cleanliness = 100f;
        protected readonly float cleanlinessMax = 100f;
        protected float endurance = 100f;
        protected readonly float enduranceMax = 100f;

        public float Hunger => hunger;
        public float Happiness => happiness;
        public float Cleanliness => cleanliness;
        public float Endurance => endurance;

        public bool InitBestCondition(bool changeHunger=true, bool changeHappiness=true, 
        bool changeCleanliness=true, bool changeEndurance=true) {
            bool changed = false;

            if (changeHunger && hunger < hungerMax) { hunger = hungerMax; changed = true; }
            if (changeHappiness && happiness < happinessMax) { happiness = happinessMax; changed = true; }
            if (changeCleanliness && cleanliness < cleanlinessMax) { cleanliness = cleanlinessMax; changed = true; }
            if  (changeEndurance && endurance < enduranceMax) { endurance = enduranceMax; changed = true; }

            return changed;
        }

        public void DecrementAll(float amount) {
            hunger = Mathf.Max(0, hunger - amount);
            happiness = Mathf.Max(0, happiness - amount);
            cleanliness = Mathf.Max(0, cleanliness - amount);
            endurance = Mathf.Max(0, endurance - amount);
        }

        public void Feed(float amount) {
            hunger = Mathf.Min(100, hunger + amount);
        }

        public void Clean(float amount) {
            cleanliness = Mathf.Min(100, cleanliness + amount);
        }

        public void Cheer(float amount) {
            happiness = Mathf.Min(100, happiness + amount);
        }
    }
}
