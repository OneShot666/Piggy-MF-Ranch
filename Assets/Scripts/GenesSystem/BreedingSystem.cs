using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Système de reproduction des cochons, gère la génération d'un cochon descendant
/// avec ses couleurs, raretés, statistiques et pouvoirs spéciaux/uniques.
/// </summary>
public class BreedingSystem : MonoBehaviour
{
    #region Constants and Weights
    
    /// <summary>
    /// Poids de chaque rareté pour le calcul de l'apparition des cochons.
    /// </summary>
    private static readonly Dictionary<PigRarity, float> rarityWeights = new Dictionary<PigRarity, float>
    {
        { PigRarity.Common, 10f },
        { PigRarity.Uncommon, 8f },
        { PigRarity.Rare, 6f },
        { PigRarity.Legendary, 4f },
        { PigRarity.UltraRare, 2f }
    };
    
    /// <summary>
    /// Poids des pouvoirs spéciaux pour le tirage aléatoire.
    /// </summary>
    private readonly (PigSpecialPower power, float weight)[] specialPowerWeights = new[]
    {
        (PigSpecialPower.None, 10f),
        (PigSpecialPower.Sprint, 8f),
        (PigSpecialPower.FatigueResist, 5f),
        (PigSpecialPower.MatingChance, 3f),
        (PigSpecialPower.XPBoost, 4f),
        (PigSpecialPower.GeneralBoost, 2f),
        (PigSpecialPower.Random, 1f)
    };

    /// <summary>
    /// Poids des pouvoirs uniques pour le tirage aléatoire.
    /// </summary>
    private readonly (PigUniquePower power, float weight)[] uniquePowerWeights = new[]
    {
        (PigUniquePower.None, 10f),
        (PigUniquePower.Sprint, 8f),
        (PigUniquePower.SlipperyMud, 6f),
        (PigUniquePower.Confusion, 7f)
    };
    
    /// <summary>
    /// Chance qu'une mutation se produise lors de la reproduction.
    /// </summary>
    [SerializeField, Range(0f, 1f)]
    private float mutationChance = 0.03f;
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Tente de faire se reproduire deux cochons et retourne le descendant.
    /// </summary>
    /// <param name="parent1">Premier parent.</param>
    /// <param name="parent2">Second parent.</param>
    /// <returns>Un nouveau cochon descendant, ou null si la reproduction n'est pas possible.</returns>
    public Pig Breed(Pig parent1, Pig parent2) {
        if (!CanBreed(parent1, parent2)) return null;
        
        bool isMutation;
        PigColor color = GetOffspringColor(parent1, parent2, out isMutation);
        PigRarity rarity = GetOffspringRarity(parent1, parent2);
        
        float speed = GetOffspringStat(parent1.Speed, parent2.Speed, rarity);
        float endurance = GetOffspringStat(parent1.Endurance, parent2.Endurance, rarity);
        
        if (isMutation)
        {
            speed *= 1.15f;
            endurance *= 1.15f;
            
            /*// Chance de booster la rareté d’un cran (sauf UltraRare)
            if (Random.value < 0.25f && rarity != PigRarity.UltraRare)
                rarity = (PigRarity)((int)rarity + 1);

            // Tirage d’un pouvoir unique supplémentaire aléatoire
            uniquePower = GetRandomPowerFromWeights(uniquePowerWeights);*/
        }
        
        PigSpecialPower specialPower = GetOffspringSpecialPower(parent1.SpecialPower, parent2.SpecialPower, rarity, color, isMutation);
        PigUniquePower uniquePower = GetOffspringUniquePower(parent1.UniquePower, parent2.UniquePower, rarity, color, isMutation);
        
        int generation = Mathf.Max(parent1.Generation, parent2.Generation) + 1;

        return new Pig(color, rarity, speed, endurance, specialPower, uniquePower, generation);
    }

    /// <summary>
    /// Vérifie si deux cochons peuvent se reproduire.
    /// </summary>
    /// <param name="parent1">Premier parent.</param>
    /// <param name="parent2">Second parent.</param>
    /// <returns>True si la reproduction est possible.</returns>
    public bool CanBreed(Pig parent1, Pig parent2) {
        return parent1.WellBeing != null && parent2.WellBeing != null &&
               // parent1.WellBeing.Satiety > 50f && parent2.WellBeing.Satiety > 50f &&
               parent1.WellBeing.Happiness >= 50f && parent2.WellBeing.Happiness >= 50f;

    }
    
    #endregion
    
    #region Private Methods
    
    [ContextMenu("Test")]
    private void Test()
    {
        Debug.Log("-------------Test Breeding-----------------");
        Pig parent1 = new Pig(PigColor.Black, PigRarity.Common, 10f, 10f, PigSpecialPower.Sprint, PigUniquePower.None,
            1);
        Pig parent2 = new Pig(PigColor.White, PigRarity.Rare, 10f, 10f, PigSpecialPower.Sprint, PigUniquePower.None,
            1);
        
        Pig pig = Breed(parent1, parent2);
        
        Debug.Log("-------------Final Breeding-----------------");
        Debug.Log(pig.Color);
        Debug.Log(pig.Rarity);
        Debug.Log(pig.Speed);
        Debug.Log(pig.Endurance);
        Debug.Log(pig.SpecialPower);
        Debug.Log(pig.UniquePower);
        Debug.Log(pig.Generation);
        Debug.Log("---------------Fin Breeding-----------------");
    }

    /// <summary>
    /// Calcule la couleur du descendant en tenant compte des mutations et combinaisons spéciales.
    /// </summary>
    private PigColor GetOffspringColor(Pig parent1, Pig parent2, out bool isMutation) {
        isMutation = parent1.Color == PigColor.Rainbow ||
                     parent2.Color == PigColor.Rainbow ||
                     Random.value < mutationChance;

        if (isMutation) return Random.value > 0.5f ? parent1.Color : parent2.Color;

        if (parent1.Color == parent2.Color) return parent1.Color;

        if (((parent1.Color == PigColor.Pink && parent2.Color == PigColor.Brown) ||
            (parent2.Color == PigColor.Pink && parent1.Color == PigColor.Brown)) &&
            Random.value < 0.5f)
            return PigColor.Beige;
        
        if (((parent1.Color == PigColor.Black && parent2.Color == PigColor.White) ||
            (parent2.Color == PigColor.Black && parent1.Color == PigColor.White)) &&
            Random.value < 0.5f)
            return PigColor.Grey;

        if (((parent1.Color == PigColor.Golden && parent2.Color == PigColor.Black) ||
            (parent2.Color == PigColor.Golden && parent1.Color == PigColor.Black)) &&
            Random.value < 0.5f)
            return PigColor.DarkGold;

        return Random.value > 0.5f ? parent1.Color : parent2.Color;
    }
    
    /// <summary>
    /// Calcule la rareté du descendant selon les parents et la propreté.
    /// </summary>
    private PigRarity GetOffspringRarity(Pig parent1, Pig parent2) {
        PigRarity[] rarities = (PigRarity[])System.Enum.GetValues(typeof(PigRarity));
        float[] weights = new float[rarities.Length];
        
        float avgScore = (rarityWeights[parent1.Rarity] + rarityWeights[parent2.Rarity]) / 2f;
        float cleanlinessFactor = Mathf.Clamp01((parent1.WellBeing.Cleanliness + parent2.WellBeing.Cleanliness) / 200f);
        
        // Déterminer la rareté max autorisée : 1 rank au-dessus de la plus haute des parents
        int maxRarityIndex = Mathf.Min(Mathf.Max((int)parent1.Rarity, (int)parent2.Rarity) + 1, rarities.Length - 1);
        
        for (int i = 0; i <= maxRarityIndex; i++)
        {
            float distance = Mathf.Abs(rarityWeights[rarities[i]] - avgScore);
            weights[i] = Mathf.Max(0.05f, 1f / (1f + distance)) * cleanlinessFactor;
        }
        
        // Les raretés supérieures au max sont interdites → poids = 0
        for (int i = maxRarityIndex + 1; i < rarities.Length; i++) {
            weights[i] = 0f;
        }
        
        return WeightedRandom(rarities, weights);
    }

    /// <summary>
    /// Calcule la valeur d'une statistique du descendant selon la rareté.
    /// </summary>
    private float GetOffspringStat(float stat1, float stat2, PigRarity rarity) {
        float baseStat = (stat1 + stat2) / 2f;

        float multiplier = rarity switch
        {
            PigRarity.Uncommon => 1.2f,
            PigRarity.Rare => 1.4f,
            PigRarity.Legendary => 1.7f,
            PigRarity.UltraRare => 2f,
            _ => 1f
        };

        return baseStat * multiplier;
    }
    
    /// <summary>
    /// Détermine le pouvoir spécial du descendant selon les parents et la mutation.
    /// </summary>
    private PigSpecialPower GetOffspringSpecialPower(PigSpecialPower specialPower1, PigSpecialPower specialPower2, PigRarity rarity, PigColor color, bool isMutation)
    {
        if (isMutation || color == PigColor.Rainbow)
            return GetRandomPowerFromWeights(specialPowerWeights);
        
        if (specialPower1 == specialPower2 && specialPower1 != PigSpecialPower.None)
            return Random.value < 0.5f ? specialPower1 : GetRandomPowerFromWeights(specialPowerWeights);
        
        return specialPower1 == PigSpecialPower.None ? specialPower2 :
            specialPower2 == PigSpecialPower.None ? specialPower1 :
            (Random.value > 0.5f ? specialPower1 : specialPower2);
    }

    /// <summary>
    /// Détermine le pouvoir unique du descendant selon les parents et la mutation.
    /// </summary>
    private PigUniquePower GetOffspringUniquePower(PigUniquePower uniquePower1, PigUniquePower uniquePower2, PigRarity rarity, PigColor color, bool isMutation)
    {
        if (isMutation || color == PigColor.Rainbow)
            return GetRandomPowerFromWeights(uniquePowerWeights);
        
        if (uniquePower1 == uniquePower2 && uniquePower1 != PigUniquePower.None)
            return Random.value < 0.5f ? uniquePower1 : GetRandomPowerFromWeights(uniquePowerWeights);

        return uniquePower1 == PigUniquePower.None ? uniquePower2 :
            uniquePower2 == PigUniquePower.None ? uniquePower1 :
            (Random.value > 0.5f ? uniquePower1 : uniquePower2);
    }
    
    /// <summary>
    /// Sélectionne un élément aléatoire parmi une liste pondérée.
    /// </summary>
    private T GetRandomPowerFromWeights<T>((T power, float weight)[] items)
    {
        float totalWeight = 0f;
        foreach (var item in items) totalWeight += item.weight;

        float rand = Random.value * totalWeight;
        float cumulative = 0f;

        foreach (var item in items)
        {
            cumulative += item.weight;
            if (rand <= cumulative) return item.power;
        }

        return items[0].power;
    }
    
    /// <summary>
    /// Sélectionne un élément aléatoire parmi une liste avec des poids spécifiques.
    /// </summary>
    private T WeightedRandom<T>(T[] items, float[] weights)
    {
        float total = 0f;
        foreach (var w in weights) total += w;

        float rand = Random.Range(0f, total);
        float cumulative = 0f;

        for (int i = 0; i < items.Length; i++)
        {
            cumulative += weights[i];
            if (rand <= cumulative) return items[i];
        }

        return items[0];
    }
    
    #endregion
}
