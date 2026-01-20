using UnityEngine;
using System;
//TODO:
// Satiety is the energy used to take part in a race: not done
// L'enclos se sali plus vite en fonction du nombre de cochons: not done
// Si le cochon a faim il perd en vitesse: not done
// Une barre de vie qui fait mourir le cochon quand elle atteint 0 et qui s'active quand satiety ou cleanliness = 0: not done

[System.Serializable]
public class WellBeing {
    
    public Action OnValuesChanged;

    private void Notify()
    {
        OnValuesChanged?.Invoke();
    }
    
    [Header("Values of gauges on 100")]
    public float Happiness = 100f;
    public float Satiety = 100f;
    public float CleanlinessPig = 100f;
    public float CleanlinessEnclosure = 100f;
    
    [Header("Duration (in-game days) before reaching 0")]
    [Range(0.1f, 30f)] public float HappinessDays = 2f;
    [Range(0.1f, 30f)] public float SatietyDays = 3f;
    [Range(0.1f, 30f)] public float CleanPigDays = 7f;
    [Range(0.1f, 30f)] public float CleanEnclosureDays = 5f;
    
    [Header("Time settings")]
    public const float SecondsPerDay = 1440f; // 24 minutes per in-game day
    
    private float SatietyDecreasePerSec => 100f / (SatietyDays * SecondsPerDay);
    private float CleanPigDecreasePerSec => 100f / (CleanPigDays * SecondsPerDay);
    private float CleanEnclosureDecreasePerSec => 100f / (CleanEnclosureDays * SecondsPerDay);
    // private float HappinessDecreasePerSec => 100f / (HappinessDays * SecondsPerDay);

    public void Decrement(float deltaTime) {
        Satiety = Mathf.Max(0, Satiety - SatietyDecreasePerSec * deltaTime);
        // Happiness = Mathf.Max(0, Happiness - HappinessDecrease * Time.deltaTime); Fais baisser la barre trop vite.
        CleanlinessPig = Mathf.Max(0, CleanlinessPig - CleanPigDecreasePerSec * deltaTime);
        CleanlinessEnclosure = Mathf.Max(0, CleanlinessEnclosure - CleanEnclosureDecreasePerSec * deltaTime);
        
        Notify();
    }

    public void HappinessIncrease(float  deltaTime)
    {
        float decreaseHappinessPerSecond = 100f / (HappinessDays * SecondsPerDay);
        float increaseHappinessPerSecond = decreaseHappinessPerSecond / 2f;
        
        bool stomachFull = Satiety >= 60f;
        bool stomachEmpty = Satiety < 40f;

        bool isClean = CleanlinessPig >= 60f && CleanlinessEnclosure >= 60f;
        bool isDirty = CleanlinessPig < 40f || CleanlinessEnclosure < 40f;

        if (Happiness >= 100f && stomachFull && isClean) return;
    	if (Happiness <= 0f && (stomachEmpty || isDirty)) return;

    	if (stomachEmpty || isDirty)
        	Happiness = Mathf.Max(0, Happiness - decreaseHappinessPerSecond * deltaTime);
    	else if (stomachFull && isClean)
        	Happiness = Mathf.Min(100, Happiness + increaseHappinessPerSecond * deltaTime);
        
        Notify();
    }

    public void Feed(float food) {
        Satiety = Mathf.Min(100, Satiety + food);
        Notify();
    }
    public void CleanPig(float amount) {
        CleanlinessPig = Mathf.Min(100, CleanlinessPig + amount);
        Notify();
    }
    public void CleanEnclosure(float amount)
    {
        CleanlinessEnclosure = Mathf.Min(100, CleanlinessEnclosure + amount);
        Notify();
    }
    public void Cheer(float amount) {
        Happiness = Mathf.Min(100, Happiness + amount);
        Notify();
    }
}