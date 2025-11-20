using UnityEngine;

public class BreedingSystem : MonoBehaviour
{
    public Pig Breed(Pig parent1, Pig parent2)
    {
        if (!CanBreed(parent1, parent2)) return null;
        // Color mutation logic
        string color = GetOffspringColor(parent1.Color, parent2.Color);
        PigRarity rarity = GetOffspringRarity(parent1.Rarity, parent2.Rarity);
        float speed = GetOffspringStat(parent1.Speed, parent2.Speed, rarity, "Speed");
        float endurance = GetOffspringStat(parent1.Endurance, parent2.Endurance, rarity, "Endurance");
        PigPower power = GetOffspringPower(parent1.SpecialPower, parent2.SpecialPower, color);
        int generation = Mathf.Max(parent1.Generation, parent2.Generation) + 1;
        return new Pig(color, rarity, speed, endurance, power, generation);
    }

    public bool CanBreed(Pig parent1, Pig parent2)
    {
        return parent1.WellBeing != null && parent2.WellBeing != null &&
               parent1.WellBeing.Happiness > 50f && parent2.WellBeing.Happiness > 50f;
    }

    private string GetOffspringColor(string color1, string color2)
    {
        // Example mutation logic
        if ((color1 == "Rose" && color2 == "Arc-en-ciel") || (color2 == "Rose" && color1 == "Arc-en-ciel"))
            return "Mutation";
        if (color1 == color2) return color1;
        if ((color1 == "Noir" && color2 == "Blanc") || (color2 == "Noir" && color1 == "Blanc"))
            return "Gris";
        // Add more combinations as needed
        return Random.value > 0.5f ? color1 : color2;
    }

    private PigRarity GetOffspringRarity(PigRarity r1, PigRarity r2)
    {
        // Simple logic: higher rarity chance if parents are rare
        if (r1 == PigRarity.UltraRare || r2 == PigRarity.UltraRare)
            return PigRarity.UltraRare;
        if (r1 == PigRarity.Legendary || r2 == PigRarity.Legendary)
            return Random.value > 0.7f ? PigRarity.Legendary : PigRarity.Rare;
        if (r1 == PigRarity.Rare || r2 == PigRarity.Rare)
            return Random.value > 0.5f ? PigRarity.Rare : PigRarity.Uncommon;
        if (r1 == PigRarity.Uncommon || r2 == PigRarity.Uncommon)
            return PigRarity.Uncommon;
        return PigRarity.Common;
    }

    private float GetOffspringStat(float stat1, float stat2, PigRarity rarity, string statType)
    {
        float baseStat = (stat1 + stat2) / 2f;
        // Apply rarity bonus
        switch (rarity)
        {
            case PigRarity.Rare:
                baseStat *= 1.1f;
                break;
            case PigRarity.Legendary:
                baseStat *= 1.2f;
                break;
            case PigRarity.UltraRare:
                baseStat *= 1.3f;
                break;
        }
        return baseStat;
    }

    private PigPower GetOffspringPower(PigPower p1, PigPower p2, string color)
    {
        if (color == "Arc-en-ciel" || color == "Mutation")
            return PigPower.Random;
        if (p1 == p2) return p1;
        if (p1 == PigPower.None) return p2;
        if (p2 == PigPower.None) return p1;
        // Randomly pick one
        return Random.value > 0.5f ? p1 : p2;
    }
}