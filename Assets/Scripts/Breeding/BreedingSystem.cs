using System.Collections.Generic;
using Breeding.Colors;
using UnityEngine;
using Pigs;

// Public enums
public enum PigActivePower { None, Sprint, SlipperyMud, Confusion }
public enum PigPassivePower { None, Sprint, FatigueResist, MatingChance, XpBoost, GeneralBoost }
public enum PigRarity { Common, Uncommon, Rare, Legendary, Unique }
public enum PigColor { Pink, Brown, Black, White, Golden, Rainbow, Grey, Beige, DarkGold }

namespace Breeding {
    /// <summary> Full pig breeding system. Manage genetic, colors, stats and rarities. </summary>
    public class BreedingSystem : MonoBehaviour {
        [Header("Probability Configuration")]
        [SerializeField, Range(0f, 1f)] private float mutationChance = 0.03f;

        [Header("Special Colour Recipes")]
        [Tooltip("Defines crosses that produce unique colors (e.g., Pink + Brown = Beige)")]
        [SerializeField] private List<PassiveColorRecipeData> specialRecipes = new();

        [Header("Color affinities with special powers")]
        [SerializeField] private List<ColorPowerAffinityData> colorPowerAffinities = new();

        [Header("Rarity Data Settings")]
        [SerializeField] private List<RarityData> raritySettings = new();

        [Header("Database")]
        [Tooltip("Each scriptables objects of pigs")]
        [SerializeField] private List<PigData> pigDatabase = new(); 

        #region Structures et Dictionnaires
    
        /// <summary> Weight of special powers for random draw. </summary>
        private readonly (PigPassivePower power, float weight)[] _passivePowerCoeffs = {
            (PigPassivePower.None, 10f), (PigPassivePower.Sprint, 8f),
            (PigPassivePower.FatigueResist, 5f), (PigPassivePower.MatingChance, 3f),
            (PigPassivePower.XpBoost, 4f), (PigPassivePower.GeneralBoost, 2f)
        };

        /// <summary> Weight of unique powers for random draw. </summary>
        private readonly (PigActivePower power, float weight)[] _uniquePowerWeights = {
            (PigActivePower.None, 10f), (PigActivePower.Sprint, 8f),
            (PigActivePower.SlipperyMud, 6f), (PigActivePower.Confusion, 7f)
        };
        #endregion

        #region Public Methods
        /// <summary> Attempt to breed two pigs and turn offspring over. </summary>
        /// <param name="parent1">First parent.</param>
        /// <param name="parent2">Second parent.</param>
        /// <returns> A new piglet offspring, or null if reproduction is not possible. </returns>
        public Pig Breed(Pig parent1, Pig parent2) {
            if (!CanBreed(parent1, parent2)) return null;

            bool isMutation = parent1.SkinColor == PigColor.Rainbow || parent2.SkinColor == PigColor.Rainbow || 
                Random.value < mutationChance;

            PigColor offspringColor = GetOffspringColor(parent1, parent2, isMutation);
            Sprite babyIcon = GetSpriteForColor(offspringColor);
            PigRarity offspringRarity = GetOffspringRarity(parent1, parent2);

            float speed = CalculateStat(parent1.Speed, parent2.Speed, offspringRarity, isMutation);

            PigPassivePower? passivePower = GetChildPassivePower(parent1.PassivePower, parent2.PassivePower, offspringColor, isMutation);
            PigActivePower? activePower = GetChildActivePower(parent1.ActivePower, parent2.ActivePower, isMutation);

            int mutationBonus = Mathf.Max(parent1.MutationBonus, parent2.MutationBonus);
            int generation = Mathf.Max(parent1.Generation, parent2.Generation) + 1;

            Pig piglet = new Pig(babyIcon, offspringColor, offspringRarity, speed, 
                passivePower, activePower, mutationBonus, generation);
            if (passivePower == null || activePower == null) piglet.FailedToGainPower();
            return piglet;
        }

        /// <summary> Check if two pigs can reproduce. </summary>
        /// <param name="parent1">Premier parent.</param>
        /// <param name="parent2">Second parent.</param>
        /// <returns>True si la reproduction est possible.</returns>
        private bool CanBreed(Pig parent1, Pig parent2) {
            return parent1 != null && parent2 != null && parent1.IsFitForBreeding() && parent2.IsFitForBreeding();
        }

        /// <summary> Find the pig image based on its image </summary>
        /// <param name="color">Color of pig</param>
        /// <returns>Icon of pig</returns>
        private Sprite GetSpriteForColor(PigColor color) {
            PigData data = pigDatabase.Find(p => p.color == color);
            return data ? data.icon : null;
        }
        #endregion

        #region Private Methods
        /// <summary> Helper to get ScriptableObject data for a specific rarity. </summary>
        private RarityData GetRarityData(PigRarity rarity) {
            RarityData data = raritySettings.Find(r => r.rarity == rarity);
            if (!data) return null;
            return data;
        }
        #endregion

        #region Helpers
        public string GetPigDataNameForColor(PigColor color) {
            PigData data = pigDatabase.Find(p => p.color == color);
            return data ? data.pigName : "Pinky";                               // Default name
        }

        /// <summary> Calculate color of offspring, taking into account mutations and special combinations. </summary>
        private PigColor GetOffspringColor(Pig parent1, Pig parent2, bool isMutation) {
            PigColor[] allColors = (PigColor[])System.Enum.GetValues(typeof(PigColor));
            float[] weights = new float[allColors.Length];

            float standardWeight = 20f;                                         // Total percentage of standard colors
            float parentBonus = 40f;                                            // Parental influence
            float recipeBonus = 50f;                                            // Top priority to recipes
        
            for (int i = 0; i < allColors.Length; i++) {
                PigColor currentColor = allColors[i];
        
                weights[i] = IsStandardColor(currentColor) ? standardWeight : 0f;   // BASIC CHANCE (Standard colors only)

                if (currentColor == parent1.SkinColor) weights[i] += parentBonus;   // Bonus from parents
                if (currentColor == parent2.SkinColor) weights[i] += parentBonus;

                foreach (var recipe in specialRecipes) {                        // Special recipes
                    if (((parent1.SkinColor == recipe.parentColorA && parent2.SkinColor == recipe.parentColorB) ||
                    (parent1.SkinColor == recipe.parentColorB && parent2.SkinColor == recipe.parentColorA)) 
                    && currentColor == recipe.result) {
                        weights[i] += recipeBonus; 
                    }
                }
            }

            if (isMutation) {                                                   // Chance to change color
                for (var i = 0; i < allColors.Length; i++) {
                    PigColor currentColor = allColors[i];

                    if (IsStandardColor(currentColor)) {                        // More chance of base colors (for "Champions")
                        weights[i] += 5f;
                    } else if (currentColor == PigColor.Rainbow) {              // Chance of Rainbow (exception)
                        weights[i] += 5f;
                    }
                }
            }

            return WeightedRandom(allColors, weights);
        }

        private bool IsStandardColor(PigColor color) {                          // If color is common
            return color is PigColor.Pink or PigColor.Brown or PigColor.Black or PigColor.White;
        }

        /// <summary> Calculate rarity of offspring based on parents and cleanliness.
        /// offspring cannot exceed 1 rank above highest rarity of parents. </summary>
        private PigRarity GetOffspringRarity(Pig parent1, Pig parent2) {
            PigRarity[] rarities = (PigRarity[])System.Enum.GetValues(typeof(PigRarity));
            float[] weights = new float[rarities.Length];

            int bestParentRarity = Mathf.Max((int)parent1.Rarity, (int)parent2.Rarity);
            // Piglet can be at most +1 compared to best parent
            int maxPossibleRarity = Mathf.Min(bestParentRarity + 1, rarities.Length - 1);

            float cleanlinessFactor = (parent1.Cleanliness + parent2.Cleanliness) / 200f;

            for (int i = 0; i < rarities.Length; i++) {
                if (i > maxPossibleRarity) {
                    weights[i] = 0f;                                            // Can't skip ranks
                } else if (i == maxPossibleRarity && i > bestParentRarity) {
                    weights[i] = 1f * cleanlinessFactor;                        // Chance of ranking up (linked to cleanliness)
                } else if (i == bestParentRarity) {
                    weights[i] = 5f;                                            // Very likely to keep same level
                } else {
                    weights[i] = 2f;                                            // Possible of regression (level down)
                }
            }

            return WeightedRandom(rarities, weights);
        }

        /// <summary> Calculates value of a descendant statistic based on rarity. </summary>
        private float CalculateStat(float stat1, float stat2, PigRarity rarity, bool mutation) {
            float average = (stat1 + stat2) / 2f;
            RarityData data = GetRarityData(rarity);
            if (!data) return average;

            float fluctuationRange = average * 0.15f;                           // piglet vary by +/- 15% around parents' average
            float randomBase = Random.Range(average - fluctuationRange * 0.5f, average + fluctuationRange);

            float baseMax = data.maxStat;
            float absoluteLimit = data.absoluteLimit;

            float rarityBonus = (int)rarity * 1.0f;                             // Bonus based on rarity
            float theoreticalStat = randomBase + rarityBonus;
            if (mutation) theoreticalStat += Random.Range(2f, 5f);              // More generous mutation

            if (theoreticalStat > baseMax) {                                    // Braking (Dynamic Soft Cap)
                float excess = theoreticalStat - baseMax;
                float accessibleRange = absoluteLimit - baseMax;
                float damping = 1f + excess / accessibleRange;                  // Softens if parents too strong
                float finalBonus = excess / damping;
        
                return baseMax + finalBonus;
            }

            float minimumSecurity = (int)rarity * 5f;                           // Unique ones don't fall below standard
            return Mathf.Max(theoreticalStat, minimumSecurity);
        }

        /// <summary> Determines special power of offspring according to parents and mutation. </summary>
        private PigPassivePower? GetChildPassivePower(PigPassivePower? passivePower1, 
        PigPassivePower? passivePower2, PigColor pigletColor,  bool isMutation) {
            if (passivePower1 == null && passivePower2 == null) return null;

            // Prepare dynamic weights (copy base weights)
            var dynamicWeights = new (PigPassivePower power, float weight)[_passivePowerCoeffs.Length];
            for (int i = 0; i < _passivePowerCoeffs.Length; i++) {
                dynamicWeights[i] = _passivePowerCoeffs[i];

                // Apply boost to ScriptableObject if color matches
                foreach (var affinity in colorPowerAffinities) {
                    if (affinity.color == pigletColor && affinity.favoredPower == dynamicWeights[i].power) {
                        dynamicWeights[i].weight *= affinity.powerBoostMultiplier;
                        break; 
                    }
                }
            }

            if (isMutation) return GetRandomPower(dynamicWeights);              // Change logic: full print run with new weights

            // Logic of heredity: if parents don't have power -> small chance of "awakening"
            if (passivePower1 == PigPassivePower.None && passivePower2 == PigPassivePower.None) {
                // 5% chance of a power appearing based on color affinities
                return Random.value < 0.05f ? GetRandomPower(dynamicWeights) : PigPassivePower.None;
            }

            // Traditional inheritance if a parent has power
            if (passivePower1 == passivePower2 && passivePower1 != PigPassivePower.None) 
                return Random.value < 0.7f ? passivePower1 : GetRandomPower(dynamicWeights);

            return Random.value > 0.5f ? passivePower1 : passivePower2;
        }

        /// <summary> Determines unique power of descendant according to parents and mutation. </summary>
        private PigActivePower? GetChildActivePower(PigActivePower? activePower1, PigActivePower? activePower2, bool isMutation) {
            if (activePower1 == null && activePower2 == null) return null;
            if (isMutation) return GetRandomPower(_uniquePowerWeights);
            if (activePower1 == activePower2 && activePower1 != PigActivePower.None) return Random.value < 0.7f ? 
                activePower1 : GetRandomPower(_uniquePowerWeights);
            return Random.value > 0.5f ? activePower1 : activePower2;
        }

        /// <summary> Selects a random item from a list with specific weights. </summary>
        private T WeightedRandom<T>(T[] items, float[] weights) {
            float total = 0;
            foreach (float w in weights) total += w;
            float r = Random.Range(0, total);
            float current = 0;
            for (int i = 0; i < items.Length; i++) {
                current += weights[i];
                if (r <= current) return items[i];
            }
            return items[0];
        }

        /// <summary> Selects a random item from a weighted list. </summary>
        private T GetRandomPower<T>((T power, float weight)[] items) {
            float total = 0;
            foreach (var item in items) total += item.weight;
            float r = Random.Range(0, total);
            float current = 0;
            foreach (var item in items) {
                current += item.weight;
                if (r <= current) return item.power;
            }
            return items[0].power;
        }
        #endregion
    }
}
