using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    [Header("Lignes")]
    public Transform startLine;   // position X = colonne de départ
    public Transform finishLine;  // position X = colonne d’arrivée

    [Header("Visuels")]
    public List<Transform> pigPrefabs; // vos 6 prefabs (sprites 2D)

    [Header("Course")]
    public float laneHeight = 10f;   // hauteur totale des 6 couloirs
    public float baseSpeed = 5f;     // vitesse moyenne

    private List<Transform> runners = new List<Transform>();
    private List<float> speeds = new List<float>();
    private bool racing = false;

    void Start() => LaunchSixPigs();

    void Update()
    {
        if (!racing) return;

        bool stillRunning = false;
        float finishX = finishLine.position.x;

        for (int i = 0; i < runners.Count; i++)
        {
            Transform t = runners[i];
            if (!t) continue;

            Vector3 p = t.position;
            p.x += speeds[i] * Time.deltaTime;
            t.position = p;

            if (p.x < finishX)
                stillRunning = true;
            else
            {
                p.x = finishX;
                t.position = p;
            }
        }

        if (!stillRunning)
        {
            racing = false;
            Debug.Log("Course terminée !");
            foreach (var t in runners) Destroy(t.gameObject);
            runners.Clear();
            speeds.Clear();
        }
    }

    void LaunchSixPigs()
    {
        runners.Clear();
        speeds.Clear();

        float stepY = laneHeight / (6 + 1);
        float bottomY = startLine.position.y - laneHeight * 0.5f;

        for (int i = 0; i < 6; i++)
        {
            Transform prefab = pigPrefabs[i % pigPrefabs.Count];

            Vector3 pos = startLine.position;
            pos.y = bottomY + stepY * (i + 1);

            Transform inst = Instantiate(prefab, pos, Quaternion.identity);
            runners.Add(inst);
            speeds.Add(baseSpeed * Random.Range(0.8f, 1.2f));
        }

        racing = true;
    }
}