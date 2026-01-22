using UnityEngine.UI;
using UnityEngine;
using Pigs;

public class UIManager : MonoBehaviour {
    public Text pigStatsText;
    public Slider hungerBar, happinessBar, cleanlinessBar;
    public Text goldText, foodText, energyText, dayText;

    public void DisplayPigStats(Pig pig) {
        if (pig == null) return;
        pigStatsText.text = $"Couleur: {pig.SkinColor}\nRarity: {pig.Rarity}\n" +
            $"Vitesse: {pig.Speed}\nEndurance: {pig.Endurance}\n" +
            $"Pouvoir: {pig.ActivePower}\nG�n�ration: {pig.Generation}";
        hungerBar.value = pig.Hunger;
        happinessBar.value = pig.Happiness;
        cleanlinessBar.value = pig.Cleanliness;
    }

    public void DisplayResources(int gold, int food, int energy, int day) {
        goldText.text = $"Or: {gold}";
        foodText.text = $"Nourriture: {food}";
        energyText.text = $"Energy: {energy}";
        dayText.text = $"Jour: {day}";
    }
    public void ShowRaceResults(Pig winner) {
        Debug.Log($"{winner.name} a won la course !");
    }
}