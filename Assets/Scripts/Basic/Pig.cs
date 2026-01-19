
// ! Make Pig a children of class WellBeing (or merge classes)
namespace Basic {
    public class Pig {
        public PigColor Color { get; }
        public PigRarity Rarity { get; }
        public float Speed { get; set; }
        public float Endurance { get; set; }
        public float MaxEndurance { get; set; }

        public PigPassivePower PassivePower { get; }
        public PigActivePower ActivePower { get; }

        public WellBeing WellBeing { get; }                                     // ?? Nani
        public int Generation { get; }                                          // For breeding lineage
        public int MutationBonus { get; }                                       // Hidden gene to gain power

        public Pig(PigColor color, PigRarity rarity, float speed, float endurance, 
            PigPassivePower passivePower, PigActivePower activePower, int generation = 1) {
            Color = color;
            Rarity = rarity;
            Speed = speed;
            MaxEndurance = endurance;
            Endurance = endurance;
            PassivePower = passivePower;
            ActivePower = activePower;
            WellBeing = new WellBeing();
            Generation = generation;
        }

        public Pig() {
            WellBeing = new WellBeing();
            Generation = 1;
        }
    }
}
