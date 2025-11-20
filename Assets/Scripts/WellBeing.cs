using UnityEngine;

public class WellBeing {
    public float Satiety = 100f;
    public float Happiness = 100f;
    public float CleanlinessPig = 100f;
    public float CleanlinessEnclosure = 100f;

    public float HungerDecrease = 0.0277f;
    public float CleanlinessDecreasePig = 0.0185f;
    public float CleanlinessDecreaseEnclosure = 0.0139f;
    public float HappinessDecrease = 0.0111f;

    public void Decrement(float deltaTime) {
        Satiety = Mathf.Max(0, Satiety - HungerDecrease * Time.deltaTime);
        Happiness = Mathf.Max(0, Happiness - HappinessDecrease * Time.deltaTime);
        CleanlinessPig = Mathf.Max(0, CleanlinessPig - CleanlinessDecreasePig * Time.deltaTime);
        CleanlinessEnclosure = Mathf.Max(0, CleanlinessEnclosure - CleanlinessDecreaseEnclosure * Time.deltaTime);
    }

    public void HappinessIncrease()
    {
        float value = 2f;

        bool stomachFull = Satiety >= 60f;
        bool stomachNeutral = Satiety < 60f && Satiety >= 40f;
        bool stomachEmpty = Satiety < 40f;

        bool IsClean = CleanlinessPig >= 60f && CleanlinessEnclosure >= 60f;
        bool IsDirty = CleanlinessPig < 40f || CleanlinessEnclosure < 40f;

        if (stomachFull && IsClean)
        {
            Happiness = Mathf.Clamp(Happiness + value, 0, 100);
        }
        else if (stomachEmpty || IsDirty)
        {
            Happiness = Mathf.Clamp(Happiness - value, 0, 100);
        }
    }

    public void Feed(float food) {
        Satiety = Mathf.Min(100, Satiety + food);
    }
    public void CleanPig(float amount) {
        CleanlinessPig = Mathf.Min(100, CleanlinessPig + amount);
    }
    public void CleanEnclosure(float amount)
    {
        CleanlinessEnclosure = Mathf.Min(100, CleanlinessEnclosure + amount);
    }
    public void Cheer(float amount) {
        Happiness = Mathf.Min(100, Happiness + amount);
    }
}