using System.Collections.Generic;
using UnityEngine;
using Basic;
using Pigs;

public class RaceManager : MonoBehaviour {
    [Header("Lignes")]
    public Transform startLine;
    public Transform finishLine;

    [Header("Visuels")]
    public List<Transform> pigPrefabs;

    [Header("Course")]
    public float laneHeight = 10f;
    public float baseSpeed = 5f;

    [Header("Aller-retour")]
    [Tooltip("2 aller-retour + 1 aller = 5 longueurs. Se termine à l'arrivée.")]
    public int totalLengths = 5;

    [Header("Joueur")]
    [Range(0, 5)] public int playerRunnerIndex = 5;
    public string playerDisplayName = "Player";

    [Header("UI")]
    [Tooltip("Bouton UI 'Start Race' à masquer pendant la course (optionnel).")]
    [SerializeField] private GameObject startRaceButton;

    [Header("UI Fin de course")]
    [SerializeField] private RaceResultsScreen resultsScreen;

    private readonly string[] aiNames = { "Julien", "Jonathan", "Gabriel", "Nathan", "Rayane" };

    private readonly List<Transform> runners = new List<Transform>();
    private readonly List<float> speedAbs = new List<float>();
    private readonly List<int> lengthsDone = new List<int>();
    private readonly List<int> direction = new List<int>();
    private readonly List<float> targetX = new List<float>();

    // Finish order (indices dans runners)
    private readonly List<int> finishOrder = new List<int>();

    private bool racing = false;

    // ⚠️ IMPORTANT:
    // Ne pas lancer la course automatiquement.
    // Le bouton UI doit appeler StartRace().

    void Update()
    {
        if (!racing) return;

        float startX = startLine.position.x;
        float finishX = finishLine.position.x;

        for (int i = 0; i < runners.Count; i++)
        {
            Transform t = runners[i];
            if (!t) continue;

            if (lengthsDone[i] >= totalLengths)
                continue;

            float dir = direction[i];
            float tx = targetX[i];

            Vector3 p = t.position;
            p.x += (speedAbs[i] * dir) * Time.deltaTime;

            bool reached = (dir > 0) ? (p.x >= tx) : (p.x <= tx);
            if (reached)
            {
                p.x = tx;
                t.position = p;

                lengthsDone[i]++;

                // Si le coureur vient de finir, on l'enregistre dans l'ordre d'arrivée
                if (lengthsDone[i] >= totalLengths)
                {
                    if (!finishOrder.Contains(i))
                        finishOrder.Add(i);

                    continue;
                }

                // Sinon on repart
                direction[i] = -direction[i];
                int newDir = direction[i];

                targetX[i] = (newDir > 0) ? finishX : startX;
                ApplyFacing(t, newDir);
            }

            // Simulate power effect
            if (pig.PassivePower == PigPassivePower.Sprint && Random.value < 0.05f) {
                duration -= 1f; // Speed boost
            } else {
                t.position = p;													// ???
            }
        }

        // Fin: tout le monde a fini
        if (finishOrder.Count >= runners.Count)
        {
            racing = false;
            OnRaceFinished();
        }
    }

    // Appelé par le bouton UI
    public void StartRace()
    {
        if (racing) return;

        if (startRaceButton != null)
            startRaceButton.SetActive(false);

        LaunchSixPigs();
        racing = true;
    }

    private void OnRaceFinished()
    {
        // rankedSprites: 1->6
        List<Sprite> rankedSprites = new List<Sprite>(runners.Count);

        int playerRank = 6;

        for (int rank = 0; rank < finishOrder.Count; rank++)
        {
            int runnerIndex = finishOrder[rank];
            Transform runner = runners[runnerIndex];

            var sr = runner != null ? runner.GetComponentInChildren<SpriteRenderer>() : null;
            rankedSprites.Add(sr != null ? sr.sprite : null);

            if (runnerIndex == playerRunnerIndex)
                playerRank = rank + 1; // 1..6
        }

        if (resultsScreen != null)
            resultsScreen.Show(rankedSprites, playerRank);

        Cleanup();
    }

    private void Cleanup()
    {
        foreach (var t in runners)
            if (t) Destroy(t.gameObject);

        runners.Clear();
        speedAbs.Clear();
        lengthsDone.Clear();
        direction.Clear();
        targetX.Clear();
        finishOrder.Clear();
    }

    private void LaunchSixPigs()
    {
        Cleanup();

        if (startLine == null || finishLine == null)
        {
            Debug.LogWarning("RaceManager: startLine/finishLine non assignées.");
            return;
        }

        if (pigPrefabs == null || pigPrefabs.Count == 0)
        {
            Debug.LogWarning("RaceManager: pigPrefabs vide.");
            return;
        }

        float stepY = laneHeight / (6 + 1);
        float bottomY = startLine.position.y - laneHeight * 0.5f;

        float startX = startLine.position.x;
        float finishX = finishLine.position.x;

        int aiNameCursor = 0;

        for (int i = 0; i < 6; i++)
        {
            Transform prefab = pigPrefabs[i % pigPrefabs.Count];

            Vector3 pos = startLine.position;
            pos.x = startX;
            pos.y = bottomY + stepY * (i + 1);

            Transform inst = Instantiate(prefab, pos, Quaternion.identity);

            runners.Add(inst);
            speedAbs.Add(baseSpeed * Random.Range(0.8f, 1.2f));
            lengthsDone.Add(0);

            direction.Add(+1);
            targetX.Add(finishX);
            ApplyFacing(inst, +1);

            // Nom (si PigRunnerUI existe)
            var ui = inst.GetComponent<PigRunnerUI>();
            if (ui == null) ui = inst.GetComponentInChildren<PigRunnerUI>(true);

            bool isPlayer = (i == playerRunnerIndex);

            string displayName = isPlayer
                ? playerDisplayName
                : aiNames[Mathf.Clamp(aiNameCursor++, 0, aiNames.Length - 1)];

            if (ui != null)
                ui.SetName(displayName, isPlayer);
        }
    }

    private void ApplyFacing(Transform t, int dir)
    {
        if (!t) return;

        var sr = t.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.flipX = (dir < 0);
            return;
        }

        Vector3 s = t.localScale;
        s.x = Mathf.Abs(s.x) * (dir > 0 ? 1f : -1f);
        t.localScale = s;
    }

    public void ShowStartButton()
    {
        if (startRaceButton != null)
            startRaceButton.SetActive(true);
    }

}
