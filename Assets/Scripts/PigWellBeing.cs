using UnityEngine;
using System;
//TODO: chance augmentée d'avoir un cochon rare si propre
// Seuils de faim et les 2 de propretés qui augmentent la joie si > 60%, entre 40 et 60 rien, en-dessous ça baisse
// Satiety is the energy used to take part in a race
// Double condition with cleanliness and rarity of the parents
// Functions to debug each actions
public class PigWellBeing : MonoBehaviour
{
    [Header("Values of gauges on 100")]
    [SerializeField] private float satiety = 100f;
    [SerializeField] private float cleanlinessPig = 100f;
    [SerializeField] private float cleanlinessEnclosure = 100f;
    [SerializeField] private float happiness = 100f;

    [Header("Rate of decrease")]
    [SerializeField] private float hungerDecrease = 0.0277f;
    [SerializeField] private float cleanlinessDecreasePig = 0.0185f;
    [SerializeField] private float cleanlinessDecreaseEnclosure = 0.0139f;
    [SerializeField] private float happinessDecrease = 0.0111f;

    [Header("Pig Events")]
    [SerializeField] private bool canReproduce = false;

    public event Action OnValuesChanged;

    private void Start()
    {
        LimitsGauges();
    }

    private void Update()
    {
        DecreaseOverTime();
        HappinessIncrease();

        OnValuesChanged?.Invoke();
    }

    private void DecreaseOverTime()
    {
        // Decrease of gauges over time
        satiety -= hungerDecrease * Time.deltaTime;
        cleanlinessPig -= cleanlinessDecreasePig * Time.deltaTime;
        cleanlinessEnclosure -= cleanlinessDecreaseEnclosure * Time.deltaTime;
        happiness -= happinessDecrease * Time.deltaTime;
    }

    private void LimitsGauges()
    {
        // Limits of gauges, 0 to 100
        satiety = Mathf.Clamp(satiety, 0f, 100f);
        cleanlinessPig = Mathf.Clamp(cleanlinessPig, 0f, 100f);
        cleanlinessEnclosure = Mathf.Clamp(cleanlinessEnclosure, 0f, 100f);
        happiness = Mathf.Clamp(happiness, 0f, 100f);
    }

    private void FeedThePig(float food)
    {
        satiety = Mathf.Clamp(satiety + food, 0f, 100f);
        OnValuesChanged?.Invoke();
    }

    private void PetThePig()
    {
        int amount = 5;
        happiness = Mathf.Clamp(happiness + amount, 0f, 100f);
        OnValuesChanged?.Invoke();
    }

    private void CleanThePig()
    {
        int amount = 5;
        cleanlinessPig = Mathf.Clamp(cleanlinessPig + amount, 0f, 100f);
        OnValuesChanged?.Invoke();
    }

    private void CleanTheEnclosure()
    {
        int amount = 5;
        cleanlinessEnclosure = Mathf.Clamp(cleanlinessEnclosure + amount, 0f, 100f);
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
        float value = 2f;

        bool stomachFull = satiety >= 60f;
        bool stomachNeutral = satiety < 60f && satiety >= 40f;
        bool stomachEmpty = satiety < 40f;

        bool IsClean = cleanlinessPig >= 60f && cleanlinessEnclosure >= 60f;
        bool IsDirty = cleanlinessPig < 40f || cleanlinessEnclosure < 40f;

        if (stomachFull && IsClean)
        {
            happiness = Mathf.Clamp(happiness + value, 0, 100);
        }
        else if (stomachEmpty || IsDirty)
        {
            happiness = Mathf.Clamp(happiness - value, 0, 100);
        }

        OnValuesChanged?.Invoke();
    }

    private void PigRarity()
    {
        switch (cleanlinessPig)
        {
            case <= 100 and > 80:
                // Rarity percentages ex:
                // UltraRare 10%
                // Legendary 30%
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