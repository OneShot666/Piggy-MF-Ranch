using UnityEngine;
using System.Collections.Generic;

public class RaceManager : MonoBehaviour {
    public List<Pig> racePigs = new List<Pig>();
    public List<Transform> pigObjects = new List<Transform>(); // Assign pig GameObjects in inspector
    public Transform startPoint, endPoint;
    public List<string> obstacles = new List<string>(); // e.g. "mud", "fence", "rock"

    public void StartRace() {
        for (int i = 0; i < racePigs.Count && i < pigObjects.Count; i++) {
            StartCoroutine(MovePig(pigObjects[i], racePigs[i]));
        }
    }

    private System.Collections.IEnumerator MovePig(Transform pigTransform, Pig pig) {
        float distance = Vector3.Distance(startPoint.position, endPoint.position);
        float duration = distance / pig.Speed;
        float elapsed = 0f;
        Vector3 start = startPoint.position;
        bool hitObstacle = false;
        while (elapsed < duration) {
            // Simulate obstacle effect
            if (!hitObstacle && obstacles.Count > 0 && Random.value < 0.1f) {
                hitObstacle = true;
                duration += 2f; // Slow down
            }
            // Simulate power effect
            if (pig.SpecialPower == PigPower.Sprint && Random.value < 0.05f) {
                duration -= 1f; // Speed boost
            }
            pigTransform.position = Vector3.Lerp(start, endPoint.position, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        pigTransform.position = endPoint.position;
    }

    public Pig GetWinner() {
        if (racePigs.Count == 0) return null;
        Pig winner = racePigs[0];
        foreach (var pig in racePigs) {
            if (pig.Speed > winner.Speed) winner = pig;
        }
        return winner;
    }
}
