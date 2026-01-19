using Basic;
using UnityEngine;
using UnityEngine.UI;

namespace Managers {
    public class UIManager : MonoBehaviour {
        public Text pigStatsText;
        public Slider hungerBar, happinessBar, cleanlinessBar, enduranceBar;
        public Text goldText, foodText, energyText, dayText;

        public void DisplayPigStats(Pig pig) {
            if (pig == null) return;
            pigStatsText.text = $"Couleur: {pig.Color}\nRareté: {pig.Rarity}\nVitesse: {pig.Speed}\n" +
                $"Pouvoir: {pig.PassivePower}\nGénération: {pig.Generation}";
            hungerBar.value = pig.WellBeing.Hunger;
            happinessBar.value = pig.WellBeing.Happiness;
            cleanlinessBar.value = pig.WellBeing.Cleanliness;
            enduranceBar.value = pig.WellBeing.Endurance;
        }

        public void DisplayResources(int gold, int food, int energy, int day) {
            goldText.text = $"Or: {gold}";
            foodText.text = $"Nourriture: {food}";
            energyText.text = $"Énergie: {energy}";
            dayText.text = $"Jour: {day}";
        }
    }
}
