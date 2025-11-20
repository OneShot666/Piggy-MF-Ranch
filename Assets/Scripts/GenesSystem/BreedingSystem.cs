using UnityEngine;

// Système de 
public class BreedingSystem : MonoBehaviour {

    public Pig Breed(Pig parent1, Pig parent2) {
        if (!CanBreed(parent1, parent2)) return null;

        // Héritage de couleur
        PigColor color = GetOffspringColor(parent1.Color, parent2.Color);

        // Héritage de rareté
        PigRarity rarity = GetOffspringRarity(parent1, parent2);

        // Statistique héritées + bonus de rareté
        float speed = GetOffspringStat(parent1.Speed, parent2.Speed, rarity, "Speed");
        float endurance = GetOffspringStat(parent1.Endurance, parent2.Endurance, rarity, "Endurance");

        // Héritage des pouvoirs
        PigSpecialPower specialPower = GetOffspringSpecialPower(parent1.SpecialPower, parent2.SpecialPower, color);
        PigUniquePower uniquePower = GetOffspringUniquePower(parent1.UniquePower, parent2.UniquePower, color);

        // Génération
        int generation = Mathf.Max(parent1.Generation, parent2.Generation) + 1;

        return new Pig(color, rarity, speed, endurance, specialPower, uniquePower, generation);
    }

    public bool CanBreed(Pig parent1, Pig parent2) {
        return parent1.WellBeing != null && parent2.WellBeing != null &&
               /*parent1.WellBeing.Satiety > 50f && parent2.WellBeing.Satiety > 50f &&*/
               parent1.WellBeing.Happiness >= 50f && parent2.WellBeing.Happiness >= 50f;

    }

    // ----- COULEUR -----
    private PigColor GetOffspringColor(PigColor color1, PigColor color2) {
        if ((color1 == PigColor.Pink && color2 == PigColor.Rainbow) ||
            (color2 == PigColor.Pink && color1 == PigColor.Rainbow))
            return PigColor.Mutation;

        if (color1 == color2) return color1;

        if ((color1 == PigColor.Pink && color2 == PigColor.Brown) ||
            (color2 == PigColor.Pink && color1 == PigColor.Brown))
            return PigColor.Beige;

        if ((color1 == PigColor.Black && color2 == PigColor.White) ||
            (color2 == PigColor.Black && color1 == PigColor.White))
            return PigColor.Grey;

        if ((color1 == PigColor.Golden && color2 == PigColor.Black) ||
            (color2 == PigColor.Golden && color1 == PigColor.Black))
            return PigColor.DarkGold;

        return Random.value > 0.5f ? color1 : color2;
    }

    // ----- RARETE -----
    [ContextMenu("Test")]
    private void Test()
    {

    }

    private PigRarity GetOffspringRarity(Pig parent1, Pig parent2) {
        int score1 = (int)parent1.Rarity;
        int score2 = (int)parent2.Rarity;

        float avgScore = (score1 + score2) / 2f;

        // Distribution lissée autour de la moyenne
        // On définit une “force” de la probabilité pour chaque score possible
        float[] weights = new float[5];

        for (int i = 0; i < 5; i++)
        {
            float distance = Mathf.Abs((i + 1) - avgScore); // distance entre score enfant possible et moyenne
            weights[i] = Mathf.Pow(0.5f, distance); // plus la distance est grande, plus le poids diminue
        }

        // Normalisation
        float sum = 0f;
        foreach (float w in weights) sum += w;
        for (int i = 0; i < weights.Length; i++) weights[i] /= sum;

        // Tirage aléatoire
        float rand = Random.value;
        Debug.Log(rand.ToString());
        float cumulative = 0f;
        for (int i = 0; i < weights.Length; i++)
        {
            Debug.Log(weights[i]);
            cumulative += weights[i];
            if (rand <= cumulative)
                return (PigRarity)(i + 1);
        }

        return PigRarity.Common;
    }

    // ----- STATS -----
    private float GetOffspringStat(float stat1, float stat2, PigRarity rarity, string statType) {
        float baseStat = (stat1 + stat2) / 2f;

        switch (rarity) {
            case PigRarity.Uncommon:
                baseStat *= 1.1f;
                break;
            case PigRarity.Rare:
                baseStat *= 1.2f;
                break;
            case PigRarity.Legendary:
                baseStat *= 1.3f;
                break;
            case PigRarity.UltraRare:
                baseStat *= 1.4f;
                break;
        }

        return baseStat;
    }

    // ----- POUVOIR SPECIAL -----
    private PigSpecialPower GetOffspringSpecialPower(PigSpecialPower specialPower1, PigSpecialPower specialPower2, PigColor color)
    {
        if (color == PigColor.Rainbow || color == PigColor.Mutation)
            return PigSpecialPower.Random;

        if (specialPower1 == specialPower2) return specialPower2;
        if (specialPower2 == PigSpecialPower.None) return specialPower1;
        if (specialPower1 == PigSpecialPower.None) return specialPower2;

        // Randomly pick one
        return Random.value > 0.5f ? specialPower1 : specialPower2;
    }

    // ----- POUVOIR UNIQUE -----
    private PigUniquePower GetOffspringUniquePower(PigUniquePower uniquePower1, PigUniquePower uniquePower2, PigColor color) {
        if (color == PigColor.Rainbow || color == PigColor.Mutation)
            return PigUniquePower.None;
        if (uniquePower1 == uniquePower2) return uniquePower1;
        if (uniquePower1 == PigUniquePower.None) return uniquePower2;
        if (uniquePower2 == PigUniquePower.None) return uniquePower1;

        // Randomly pick one
        return Random.value > 0.5f ? uniquePower1 : uniquePower2;
    }
}
