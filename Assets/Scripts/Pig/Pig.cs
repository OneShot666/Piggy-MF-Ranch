using UnityEngine;

public enum PigRarity {
    Common,
    Uncommon,
    Rare,
    Legendary,
    UltraRare
}

public enum PigPower {
    None,
    Sprint,
    FatigueResist,
    XPBoost,
    GeneralBoost,
    Random
}

public class Pig {
    public string Color { get; set; }
    public PigRarity Rarity { get; set; }
    public float Speed { get; set; }
    public float Endurance { get; set; }
    public PigPower SpecialPower { get; set; }
    public WellBeing WellBeing { get; set; }
    public int Generation { get; set; } // For breeding lineage

    public Pig(string color, PigRarity rarity, float speed, float endurance, PigPower power, int generation = 1) {
        Color = color;
        Rarity = rarity;
        Speed = speed;
        Endurance = endurance;
        SpecialPower = power;
        WellBeing = new WellBeing();
        Generation = generation;
    }
    public Pig() {
        WellBeing = new WellBeing();
        Generation = 1;
    }
}
