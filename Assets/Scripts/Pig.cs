using UnityEngine;

public class Pig {
    public string Color { get; set; }
    public float Speed { get; set; }
    public float Endurance { get; set; }
    public string SpecialPower { get; set; }
    public WellBeing WellBeing { get; set; }

    public Pig(string color, float speed, float endurance, string specialPower) {
        Color = color;
        Speed = speed;
        Endurance = endurance;
        SpecialPower = specialPower;
        WellBeing = new WellBeing();
    }
    public Pig() {
        WellBeing = new WellBeing();
    }
}
