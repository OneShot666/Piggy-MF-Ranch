using UnityEngine;

public class GameLoop : MonoBehaviour
{
    public FarmManager farmManager;
    public PigManager pigManager;
    public float dayLength = 60f;
    private float timer = 0f;
    public int dayCount = 1;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= dayLength)
        {
            timer = 0f;
            NextDay();
        }
    }

    void NextDay()
    {
        dayCount++;
        farmManager.AddGold(10 + dayCount * 2); // Récolte quotidienne progressive
        farmManager.AddFood(5);
        farmManager.RestoreEnergy();
        // Progression: unlocks, events, etc.
        if (dayCount == 5)
        {
            // Unlock new enclosure, notify player
        }
        if (dayCount == 10)
        {
            // Unlock special food, notify player
        }
        // ...other progression logic...
    }
}