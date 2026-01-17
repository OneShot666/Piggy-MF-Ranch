using UnityEngine;

public class Pig {
    public PigColor Color { get; set; }
    public PigRarity Rarity { get; set; }
    public float Speed { get; set; }
    public float Endurance { get; set; }

    public PigSpecialPower SpecialPower { get; set; }
    public PigUniquePower UniquePower { get; set; }

    public WellBeing WellBeing { get; set; }
    public int Generation { get; set; } // For breeding lineage

    public Pig(PigColor color, PigRarity rarity, float speed, float endurance, PigSpecialPower specialPower, PigUniquePower uniquePower, int generation = 1) {
        Color = color;
        Rarity = rarity;
        Speed = speed;
        Endurance = endurance;
        SpecialPower = specialPower;
        UniquePower = uniquePower;
        WellBeing = new WellBeing();
        Generation = generation;
    }
    public Pig() {
        WellBeing = new WellBeing();
        Generation = 1;
    }
}
