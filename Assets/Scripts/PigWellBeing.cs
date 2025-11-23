using UnityEngine;
using System;
//TODO: chance augmentée d'avoir un cochon rare si propre: half done
// Seuils de faim et les 2 de propretés qui augmentent la joie si > 60%, entre 40 et 60 rien, en-dessous ça baisse: done
// Satiety is the energy used to take part in a race: not done
// Le cochon se sali plus vite en fonction de l'enclos: not done
// L'enclos se sali plus vite en fonction du nombre de cochons: not done
// Si le cochon a faim il perd en vitesse: not done
// Une barre de vie qui fait mourir le cochon quand elle atteint 0 et qui s'active quand satiety ou cleanliness = 0: not done
public class PigWellBeing : MonoBehaviour
{
    [Header("Values of gauges on 100")]
    [SerializeField] public float satiety = 100f;
    [SerializeField] public float cleanlinessPig = 100f;
    [SerializeField] public float cleanlinessEnclosure = 100f;
    [SerializeField] public float happiness = 100f;
    
    [Header("Rate of decrease")]
    [SerializeField] private float satietyDecrease = 100f / 4320f; // 3j = 72 minutes
    [SerializeField] private float cleanlinessDecreasePig = 100f / 10080f; // 7j = 168 minutes
    [SerializeField] private float cleanlinessDecreaseEnclosure = 100f / 7200f; // 5j = 120 minutes
    [SerializeField] private float happinessDecrease = 100f / 1440f; // 1j = 24 minutes
    
    [Header("Time By Day")]
    [SerializeField] private float timeByDay = 1440f; // Number of seconds by day
    [SerializeField] private float day = 1f;
    [SerializeField] private float elapsedTime = 0f;
    
    [Header("Pig Events")]
    [SerializeField] private bool canReproduce = false;
    
    public event Action OnValuesChanged;
    
    private void Update()
    {
        elapsedTime += Time.deltaTime;

        // Check for new day
        if (elapsedTime >= timeByDay)
        {
            int daysPassed = Mathf.FloorToInt(elapsedTime / timeByDay);
            day += daysPassed;
            elapsedTime -= daysPassed * timeByDay;
        }
        
        // Decrease of gauges over time
        satiety -= satietyDecrease * Time.deltaTime;
        cleanlinessPig -= cleanlinessDecreasePig * Time.deltaTime;
        cleanlinessEnclosure -= cleanlinessDecreaseEnclosure * Time.deltaTime;
        //happiness -= happinessDecrease * Time.deltaTime;
        Debug.Log($"Satiety: {satiety:F2}, PigClean: {cleanlinessPig:F2}, Enclosure: {cleanlinessEnclosure:F2}");

        
        // Limits of gauges, 0 to 100
        satiety = Mathf.Clamp(satiety, 0f, 100f);
        cleanlinessPig = Mathf.Clamp(cleanlinessPig, 0f, 100f);
        cleanlinessEnclosure = Mathf.Clamp(cleanlinessEnclosure, 0f, 100f);
        happiness = Mathf.Clamp(happiness, 0f, 100f);

        HappinessIncrease();
        
        OnValuesChanged?.Invoke();
    }

    public void FeedThePig(float amount)
    {
        satiety = Mathf.Clamp(satiety + amount, 0f, 100f);
        OnValuesChanged?.Invoke();
    }

    public void PetThePig(float amount)
    {
        happiness = Mathf.Clamp(happiness + amount, 0f, 100f);
        OnValuesChanged?.Invoke();
    }

    public void CleanThePig(float amount)
    {
        cleanlinessPig = Mathf.Clamp(cleanlinessPig + amount, 0f, 100f);
        OnValuesChanged?.Invoke();
    }

    public void CleanTheEnclosure(float amount)
    {
        cleanlinessEnclosure = Mathf.Clamp(cleanlinessEnclosure - amount, 0f, 100f);
        OnValuesChanged?.Invoke();
    }

    private void PigReproduction()
    {
        if (satiety >= 50 && happiness >= 50)
        {
            canReproduce = true;
        }
    }

    private void HappinessIncrease()
    {
        float decreaseHappinessPerSecond = 100f / timeByDay;
        float increaseHappinessPerSecond = 100f / (timeByDay * 2f);
        
        bool stomachFull = satiety >= 60f;
        bool stomachEmpty = satiety < 40f;

        bool isClean = cleanlinessPig >= 60f && cleanlinessEnclosure >= 60f;
        bool isDirty = cleanlinessPig < 40f || cleanlinessEnclosure < 40f;

        if (stomachEmpty || isDirty)
            happiness = Mathf.Clamp(happiness - decreaseHappinessPerSecond * Time.deltaTime, 0, 100);
        else if (stomachFull && isClean)
            happiness = Mathf.Clamp(happiness + increaseHappinessPerSecond * Time.deltaTime, 0, 100);
    }

    private void PigRarity()
    {
        switch (cleanlinessPig)
        {
            case <= 100 and > 80:
                // Rarity percentages ex:
                // Legendary 10%
                // Epic 30%
                // Rare 40%
                // Uncommon 15%
                // Common 5%
                break;
            
            case <= 80 and > 60:
                
                break;
            
            case <= 60 and > 40:
                
                break;
            
            case <= 40 and > 20:

                break;
            
            case <= 20 and >= 0:

                break;
        }
    }
}