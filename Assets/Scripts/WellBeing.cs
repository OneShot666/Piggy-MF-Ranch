using UnityEngine;

public class WellBeing
{
    public float Hunger = 100f;
    public float Happiness = 100f;
    public float Cleanliness = 100f;

    public void Decrement(float amount)
    {
        Hunger = Mathf.Max(0, Hunger - amount);
        Happiness = Mathf.Max(0, Happiness - amount);
        Cleanliness = Mathf.Max(0, Cleanliness - amount);
    }
    public void Feed(float amount)
    {
        Hunger = Mathf.Min(100, Hunger + amount);
    }
    public void Clean(float amount)
    {
        Cleanliness = Mathf.Min(100, Cleanliness + amount);
    }
    public void Cheer(float amount)
    {
        Happiness = Mathf.Min(100, Happiness + amount);
    }
}