using UnityEngine;
using Managers;
using PigManager = Managers.PigManager;

public class GameLoop : MonoBehaviour
{
    public FarmManager farmManager;
    public PigManager pigManager;
    public float dayLength = 60f * 24;                                          // 1h = 1 min in game
    private float _timer;
    public int dayCount = 1;                                                    // ? Add UI to show day/night count

    private void Start() {
        if (!farmManager) FindFirstObjectByType<FarmManager>();
        if (!pigManager) FindFirstObjectByType<PigManager>();
    }

<<<<<<< HEAD
    void Update() {
        _timer += Time.deltaTime;
        if (_timer >= dayLength) {
            _timer = 0f;
=======
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= dayLength)
        {
            timer = 0f;
>>>>>>> tsunaka-branch
            NextDay();
        }
    }

    void NextDay()
    {
        dayCount++;
<<<<<<< HEAD

        if (farmManager && farmManager.hasAutoHarvest) {                        // Auto-harvest
            farmManager.AddGold(10 + dayCount * 2);
            farmManager.AddFood(5);
            farmManager.RestoreEnergy();
        }

        // ? Progression: unlocks, events, etc.
        if (dayCount == 5) {
            // ? Unlock new enclosure, notify player
        } else if (dayCount == 10) {
            // ? Unlock special food, notify player
=======
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
>>>>>>> tsunaka-branch
        }
        // ...other progression logic...
    }
}