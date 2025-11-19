using UnityEngine;

public class BreedingSystem : MonoBehaviour {
    public Pig Breed(Pig parent1, Pig parent2) {
        // Condition simple : Bonheur des deux parents > 50
        if (CanBreed(parent1, parent2)) {
            string color = Random.value > 0.5f ? parent1.Color : parent2.Color;
            float speed = (parent1.Speed + parent2.Speed) / 2f;
            float endurance = (parent1.Endurance + parent2.Endurance) / 2f;
            string specialPower = Random.value > 0.5f ? parent1.SpecialPower : parent2.SpecialPower;
            return new Pig(color, speed, endurance, specialPower);
        }
        return null;
    }
    public bool CanBreed(Pig parent1, Pig parent2) {
        return parent1.WellBeing != null && parent2.WellBeing != null &&
               parent1.WellBeing.Happiness > 50f && parent2.WellBeing.Happiness > 50f;
    }
}
