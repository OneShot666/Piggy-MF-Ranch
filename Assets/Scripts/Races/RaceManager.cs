using System.Collections.Generic;
using UnityEngine;
// using Pigs;

// ReSharper disable Unity.PerformanceCriticalCodeInvocation
namespace Races {
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

        private readonly string[] _aiNames = { "Julien", "Jonathan", "Gabriel", "Nathan", "Rayane" };

        private readonly List<Transform> _runners = new();
        private readonly List<float> _speedAbs = new();
        private readonly List<int> _lengthsDone = new();
        private readonly List<int> _direction = new();
        private readonly List<float> _targetX = new();

        // Finish order (indices dans runners)
        private readonly List<int> _finishOrder = new();

        private bool _racing;

        // ⚠️ IMPORTANT :
        // Ne pas lancer la course automatiquement.
        // Le bouton UI doit appeler StartRace().

        void Update() {
            if (!_racing) return;

            float startX = startLine.position.x;
            float finishX = finishLine.position.x;

            for (int i = 0; i < _runners.Count; i++) {
                Transform t = _runners[i];
                if (!t) continue;

                if (_lengthsDone[i] >= totalLengths) continue;

                float dir = _direction[i];
                float tx = _targetX[i];

                Vector3 p = t.position;
                p.x += _speedAbs[i] * dir * Time.deltaTime;

                bool reached = dir > 0 ? p.x >= tx : p.x <= tx;
                if (reached) {
                    p.x = tx;
                    t.position = p;

                    _lengthsDone[i]++;

                    // Si le coureur vient de finir, on l'enregistre dans l'ordre d'arrivée
                    if (_lengthsDone[i] >= totalLengths) {
                        if (!_finishOrder.Contains(i)) _finishOrder.Add(i);

                        continue;
                    }

                    // Sinon on repart
                    _direction[i] = -_direction[i];
                    int newDir = _direction[i];

                    _targetX[i] = (newDir > 0) ? finishX : startX;
                    ApplyFacing(t, newDir);
                }

                // Simulate power effect
                // if (pig.PassivePower == PigPassivePower.Sprint && Random.value < 0.05f) {
                //     duration -= 1f; // Speed boost
                // } else {
                //     t.position = p;													// ???
                // }
            }

            // Fin : tout le monde a fini
            if (_finishOrder.Count >= _runners.Count) {
                _racing = false;
                OnRaceFinished();
            }
        }

        // Appelé par le bouton UI
        public void StartRace() {
            if (_racing) return;

            if (startRaceButton) startRaceButton.SetActive(false);

            LaunchSixPigs();
            _racing = true;
        }

        private void OnRaceFinished() {
            // rankedSprites: 1->6
            List<Sprite> rankedSprites = new List<Sprite>(_runners.Count);

            int playerRank = 6;

            for (int rank = 0; rank < _finishOrder.Count; rank++) {
                int runnerIndex = _finishOrder[rank];
                Transform runner = _runners[runnerIndex];

                var sr = runner ? runner.GetComponentInChildren<SpriteRenderer>() : null;
                rankedSprites.Add(sr ? sr.sprite : null);

                if (runnerIndex == playerRunnerIndex)
                    playerRank = rank + 1; // 1..6
            }

            if (resultsScreen)
                resultsScreen.Show(rankedSprites, playerRank);

            Cleanup();
        }

        private void Cleanup() {
            foreach (var t in _runners)
                if (t) Destroy(t.gameObject);

            _runners.Clear();
            _speedAbs.Clear();
            _lengthsDone.Clear();
            _direction.Clear();
            _targetX.Clear();
            _finishOrder.Clear();
        }

        private void LaunchSixPigs() {
            Cleanup();

            if (!startLine || !finishLine) {
                Debug.LogWarning("RaceManager: startLine/finishLine non assignées.");
                return;
            }

            if (pigPrefabs == null || pigPrefabs.Count == 0) {
                Debug.LogWarning("RaceManager: pigPrefabs vide.");
                return;
            }

            float stepY = laneHeight / (6 + 1);
            float bottomY = startLine.position.y - laneHeight * 0.5f;

            float startX = startLine.position.x;
            float finishX = finishLine.position.x;

            int aiNameCursor = 0;

            for (int i = 0; i < 6; i++) {
                Transform prefab = pigPrefabs[i % pigPrefabs.Count];

                Vector3 pos = startLine.position;
                pos.x = startX;
                pos.y = bottomY + stepY * (i + 1);

                Transform inst = Instantiate(prefab, pos, Quaternion.identity);

                _runners.Add(inst);
                _speedAbs.Add(baseSpeed * Random.Range(0.8f, 1.2f));
                _lengthsDone.Add(0);

                _direction.Add(+1);
                _targetX.Add(finishX);
                ApplyFacing(inst, +1);

                // Nom (si PigRunnerUI existe)
                var ui = inst.GetComponent<PigRunnerUI>();
                if (!ui) ui = inst.GetComponentInChildren<PigRunnerUI>(true);

                bool isPlayer = i == playerRunnerIndex;

                string displayName = isPlayer ? playerDisplayName
                    : _aiNames[Mathf.Clamp(aiNameCursor++, 0, _aiNames.Length - 1)];

                if (ui) ui.SetName(displayName, isPlayer);
            }
        }

        private void ApplyFacing(Transform t, int dir) {
            if (!t) return;

            var sr = t.GetComponentInChildren<SpriteRenderer>();
            if (sr) {
                sr.flipX = (dir < 0);
                return;
            }

            Vector3 s = t.localScale;
            s.x = Mathf.Abs(s.x) * (dir > 0 ? 1f : -1f);
            t.localScale = s;
        }

        public void ShowStartButton() {
            if (startRaceButton) startRaceButton.SetActive(true);
        }

    }
}
