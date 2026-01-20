using System.Collections.Generic;
using Breeding.Colors;
using UnityEngine;
using Basic;
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

            bool isMutation = parent1.Color == PigColor.Rainbow || parent2.Color == PigColor.Rainbow || 
                Random.value < mutationChance;

            PigColor offspringColor = GetOffspringColor(parent1, parent2, isMutation);
            PigRarity offspringRarity = GetOffspringRarity(parent1, parent2);

            float speed = CalculateStat(parent1.Speed, parent2.Speed, offspringRarity, isMutation);

            PigPassivePower? passivePower = GetChildPassivePower(parent1.PassivePower, parent2.PassivePower, offspringColor, isMutation);
            PigActivePower? activePower = GetChildActivePower(parent1.ActivePower, parent2.ActivePower, isMutation);

            int mutationBonus = Mathf.Max(parent1.MutationBonus, parent2.MutationBonus);
            int generation = Mathf.Max(parent1.Generation, parent2.Generation) + 1;

            Pig newPig = new Pig(offspringColor, offspringRarity, speed, passivePower, activePower, mutationBonus, generation);
            if (passivePower == null || activePower == null) newPig.FailedToGainPower();
            return newPig;
        }

        /// <summary> Check if two pigs can reproduce. </summary>
        /// <param name="parent1">Premier parent.</param>
        /// <param name="parent2">Second parent.</param>
        /// <returns>True si la reproduction est possible.</returns>
        private bool CanBreed(Pig parent1, Pig parent2) {
            return parent1 != null && parent2 != null && parent1.IsFitForBreeding() && parent2.IsFitForBreeding();
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

        #region Tests
        [ContextMenu("Analytical Lineage Test")]
        public void TestLigneeAnalytique() {
            Debug.Log("<color=orange><b>=== START OF ANALYTICAL GENEALOGY TEST ===</b></color>");

            // Initializing starting parents (Generation 0)
            Pig parentA = new Pig(PigColor.Pink, PigRarity.Common, 10f, 
                PigPassivePower.None, PigActivePower.None, 0, 0);
            Pig parentB = new Pig(PigColor.Brown, PigRarity.Common, 10f, 
                PigPassivePower.None, PigActivePower.None, 0, 0);
        
            // We force optimal conditions to see maximum potential
            parentA.InitBestCondition(false, true, true, false);
            parentB.InitBestCondition(false, true, true, false);

            for (int gen = 1; gen <= 100; gen++) {
                Debug.Log($"<color=white><b>--- GENERATION {gen} ---</b></color>");
                Debug.Log($"Parents: {parentA.Rarity} (Vit:{parentA.Speed:F1}) x {parentB.Rarity} (Vit:{parentB.Speed:F1})");

                // --- PROBABILITY CALCULATIONS ---
                PigRarity[] rarities = (PigRarity[])System.Enum.GetValues(typeof(PigRarity));
                float[] weights = new float[rarities.Length];
                int maxIndex = Mathf.Min(Mathf.Max((int)parentA.Rarity, (int)parentB.Rarity) + 1, rarities.Length - 1);
                float cleanlinessFactor = (parentA.Cleanliness + parentB.Cleanliness) / 200f;

                float totalWeight = 0;
                string probReport = "Calculated probabilities : ";

                for (int i = 0; i <= maxIndex; i++) {
                    float dist = Mathf.Abs(GetRarityData(rarities[i]).coeff - 
                        (GetRarityData(parentA.Rarity).coeff + GetRarityData(parentB.Rarity).coeff) / 2f);
                    weights[i] = 1f / (1f + dist);
                    if (i > Mathf.Max((int)parentA.Rarity, (int)parentB.Rarity)) weights[i] *= cleanlinessFactor;
                    totalWeight += weights[i];
                }

                for (int i = 0; i < rarities.Length; i++) {
                    if (i <= maxIndex) {
                        float prc = (weights[i] / totalWeight) * 100f;
                        probReport += $"| {rarities[i]}: {prc:F0}% ";
                    } else {
                        probReport += $"| <color=red>{rarities[i]}: BLOCKED</color> ";
                    }
                }
                Debug.Log(probReport);

                // --- REAL REPRODUCTION ---
                Pig piglet = Breed(parentA, parentB);

                // DISPLAYING RESULT
                string mutationText = piglet.Color != parentA.Color && piglet.Color != parentB.Color ? 
                    " <color=magenta>[NEW COLOR !]</color>" : "";
                Debug.Log($"<b>PIGLET RESULT :</b> {piglet.Color} | {piglet.Rarity} | Vit: {piglet.Speed:F2} | " +
                    $"Endurance: {piglet.Endurance:F2} |  {piglet.PassivePower} | {piglet.ActivePower} {mutationText}");

                // SELECTION FOR NEXT GENERATION
                // We always replace "least good" parent (Rarity priority then Speed)
                if ((int)parentA.Rarity < (int)parentB.Rarity || (parentA.Rarity == parentB.Rarity && parentA.Speed < parentB.Speed))
                    parentA = piglet;
                else
                    parentB = piglet;
            }

            Debug.Log("<color=orange><b>=== END OF GENEALOGY TEST ===</b></color>");
        }

        [ContextMenu("Affinity Test: Powers")]
        public void TestAffinitePouvoirs() {
            Debug.Log("<color=cyan><b>=== GENETIC LABORATORY: AFFINITY TEST ===</b></color>");

            PigColor couleurTest = PigColor.Brown;                              //  Color to test
            int nombreTests = 1000;

            // Dictionary for counting results
            Dictionary<PigPassivePower, int> resultats = new Dictionary<PigPassivePower, int>();
            foreach (PigPassivePower p in System.Enum.GetValues(typeof(PigPassivePower))) resultats[p] = 0;

            // Simulation of "empty" parents (None) to see if affinity creates awakening
            Pig p1 = new Pig(PigColor.Pink, PigRarity.Common, 10f, 
                PigPassivePower.None, PigActivePower.None, 0, 0);
            Pig p2 = new Pig(PigColor.Pink, PigRarity.Common, 10f, 
                PigPassivePower.None, PigActivePower.None, 0, 0);
            p1.InitBestCondition(false, false, true, false);
            p2.InitBestCondition(false, false, true, false);

            for (int i = 0; i < nombreTests; i++) {
                // We force mutation to see weight draws
                PigPassivePower? powerObtenu = GetChildPassivePower(p1.PassivePower, p2.PassivePower,couleurTest, true);
                if (powerObtenu != null) resultats[(PigPassivePower)powerObtenu]++;
            }

            // Displaying statistics
            Debug.Log($"<color=yellow>Results for {nombreTests} births of color {couleurTest} (Forced Mutation) :</color>");
            foreach (var entry in resultats) {
                float pourcentage = entry.Value / (float)nombreTests * 100f;
                string highlight = entry.Value > nombreTests / 5 ? "<color=green><b>(BOOSTED)</b></color>" : "";
                Debug.Log($"- {entry.Key}: {entry.Value} ({pourcentage:F1}%) {highlight}");
            }
        }

        [ContextMenu("Detailed Genetic Test")]
        public void TestGenetiqueDetaille() {
            Debug.Log("<color=cyan><b>=== DETAILED GENETIC TEST (10,000 Births) ===</b></color>");

            // Test parents: Two Roses (Standard colors)
            Pig p1 = new Pig(PigColor.Pink, PigRarity.Common, 10, 
                PigPassivePower.None, PigActivePower.None, 0, 0);
            Pig p2 = new Pig(PigColor.Pink, PigRarity.Common, 10, 
                PigPassivePower.None, PigActivePower.None, 0, 0);

            // Dictionaries for counting
            Dictionary<PigColor, int> totalByColor = new Dictionary<PigColor, int>();
            Dictionary<PigColor, int> mutationsPerColor = new Dictionary<PigColor, int>();

            foreach (PigColor c in System.Enum.GetValues(typeof(PigColor))) {
                totalByColor[c] = 0; 
                mutationsPerColor[c] = 0;
            }

            int totalTests = 10000;
            int totalMutationsActives = 0;

            for (int i = 0; i < totalTests; i++) {
                // 1. We calculate whether a mutation SHOULD occur (3% chance according to your code)
                // Note: We use same logic as in your Breed() method
                bool isMutation = Random.value < mutationChance; 
                if (isMutation) totalMutationsActives++;

                // 2. We obtain color
                PigColor colorResult = GetOffspringColor(p1, p2, isMutation);

                totalByColor[colorResult]++;
                if (isMutation) mutationsPerColor[colorResult]++;
            }

            // --- DISPLAYING RESULTS ---
            Debug.Log($"<color=yellow>Birth Report (Mutation Chance: {mutationChance*100}%)</color>");
            Debug.Log($"Total number of mutations triggered : {totalMutationsActives} / {totalTests}");

            foreach (PigColor c in System.Enum.GetValues(typeof(PigColor))) {
                if (totalByColor[c] == 0) continue;                          // Don't display colors at 0%

                float prcTotal = totalByColor[c] / (float)totalTests * 100f;
                int nbMutations = mutationsPerColor[c];
                float prcMutationDansCetteCouleur = totalByColor[c] > 0 ? nbMutations / (float)totalByColor[c] * 100f : 0;

                Debug.Log($"- <b>{c}:</b> {prcTotal:F1}% du total " +
                    $"<color=orange>[Muted: {nbMutations} ({prcMutationDansCetteCouleur:F1}% of this color)]</color>");
            }

            // RULES CHECK
            bool beigeFound = totalByColor[PigColor.Beige] > 0;
            bool goldenFound = totalByColor[PigColor.Golden] > 0;

            if (!beigeFound && !goldenFound)
                Debug.Log("<color=green>SUCCESS: <b>No recipe color (Beige/Golden) appeared by mistake..</color>");
            else
                Debug.Log("<color=red><b>ERROR: </b>Recipe colors appeared without right parents !</color>");
        }
        #endregion Tests

        #region Helpers
        /// <summary> Calculate color of offspring, taking into account mutations and special combinations. </summary>
        private PigColor GetOffspringColor(Pig parent1, Pig parent2, bool isMutation) {
            PigColor[] allColors = (PigColor[])System.Enum.GetValues(typeof(PigColor));
            float[] weights = new float[allColors.Length];

            // --- PROBABILITY SETTINGS ---
            float standardWeight = 20f;                                         // Total percentage of standard colors
            float parentBonus = 40f;                                            // Parental influence
            float recipeBonus = 50f;                                            // Top priority to recipes
        
            for (int i = 0; i < allColors.Length; i++) {
                PigColor currentColor = allColors[i];
        
                weights[i] = IsStandardColor(currentColor) ? standardWeight : 0f;   // BASIC CHANCE (Standard colors only)

                // PARENTAL BONUS (Applies even if it's a special color)
                if (currentColor == parent1.Color) weights[i] += parentBonus;
                if (currentColor == parent2.Color) weights[i] += parentBonus;

                foreach (var recipe in specialRecipes) {                        // BONUS RECIPES
                    if (((parent1.Color == recipe.parentColorA && parent2.Color == recipe.parentColorB) ||
                    (parent1.Color == recipe.parentColorB && parent2.Color == recipe.parentColorA)) 
                    && currentColor == recipe.result) {
                        weights[i] += recipeBonus; 
                    }
                }
            }

            // 4. MUTATION (Opening of all colors)
            if (isMutation) {
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
