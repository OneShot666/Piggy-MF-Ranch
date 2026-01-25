using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Breeding;
using Pigs;

// . Use real pig speed instead + random
// ! Use a mult for race (x1 by default) -> create panel
// L Add reward based on position of player's pigs (add overlay for trophy) -> make script Race.cs (scriptable object ?)
// L Find a way to play particules behind result screen objects
// LL Create different race with their own groups of pigs to beat

// ReSharper disable IteratorNeverReturns
// ReSharper disable Unity.PerformanceCriticalCodeInvocation
namespace Races {
    public class RaceManager : MonoBehaviour {
        [Header("Screens references")]
        [SerializeField] private GameObject raceScreen;
        [SerializeField] private GameObject resultsScreen;                      // Screen at the end of the race
        [SerializeField] private Image resultsTitleImage;                       // "Victory" or "Defeat" image slot
        [SerializeField] private Sprite victoryTitleSprite;                     // "Victory" image
        [SerializeField] private Sprite defeatTitleSprite;                      // "Defeat" image
        [SerializeField] private Text counterText;                              // Count lengths

        [Header("Race Layout")]
        [SerializeField] private RectTransform raceArea;
        [SerializeField] private List<RectTransform> raceSlots = new();
        [SerializeField] private RectTransform startLine;
        [SerializeField] private RectTransform finishLine;

        [Header("Race Settings")]
        [SerializeField] private GameObject pigPrefab;
        [SerializeField] private List<PigData> possiblePigsData;                // Pigs profiles
        [SerializeField] private float minBaseSpeed = 150f;
        [SerializeField] private float maxBaseSpeed = 250f;
        [SerializeField] private int totalLengths = 3;

        [Header("Player settings")]
        [SerializeField] private string playerName = "Player";
        [SerializeField] private PigData playerPigData;

        [Header("UI & FX")]
        [SerializeField] private GameObject startRaceButton;
        [SerializeField] private ParticleSystem confetti;
        [SerializeField] private ParticleSystem rain;
        [SerializeField] private Transform[] podiumSpotPigs = new Transform[6];

        [Header("Animation settings")]
        [SerializeField] private float jumpOffsetY = 50f;
        [SerializeField] private float jumpUpTime = 0.15f;
        [SerializeField] private float top3LoopPause = 0.3f;

        private readonly string[] _devsNames = { "Julien", "Jonathan", "Gabriel", "Nathan", "Rayane" };
        private readonly string[] _pigsNames = { "Groin", "Bacon", "Ham", "Truffle", "Porky" };
        private readonly string[] _dogsNames = { "Coco", "Max", "Waffle", "Lola", "Rex" };
        private readonly List<UIPigVisual> _runners = new();
        private readonly List<float> _speeds =        new();
        private readonly List<int> _lengthsDone =     new();
        private readonly List<int> _direction =       new();
        private readonly List<int> _finishOrder =     new();

        private Coroutine _endScreenCoroutine;
        private string[] _usedNames;
        private int CurrentLenght => _lengthsDone.Prepend(0).Max();
        private int _playerIndex;
        private bool _racing;

        private void Start() {
            if (raceScreen) raceScreen.SetActive(true);
            if (resultsScreen) resultsScreen.SetActive(false);                  // Hide by default
            if (counterText) counterText.text = $"Lengths : {CurrentLenght}/{totalLengths}";

            SetFx(confetti, false);
            SetFx(rain, false);
            
            CreatePigs();
        }

        private void CreatePigs() {
            _playerIndex = Random.Range(0, raceSlots.Count); 
            _usedNames = Choice(_devsNames, _pigsNames, _dogsNames);

            int aiNameCursor = 0;

            for (int i = 0; i < raceSlots.Count; i++) {
                GameObject inst = Instantiate(pigPrefab, raceArea);
                UIPigVisual visual = inst.GetComponent<UIPigVisual>();
                RectTransform rt = inst.GetComponent<RectTransform>();

                rt.anchoredPosition = raceSlots[i].anchoredPosition;
                rt.sizeDelta = raceSlots[i].sizeDelta;

                bool isPlayer = i == _playerIndex;                              // Create player's pig or a random pig
                PigData data = isPlayer ? playerPigData : possiblePigsData[Random.Range(0, possiblePigsData.Count)];
                string dName = isPlayer ? playerName : _usedNames[Mathf.Clamp(aiNameCursor++, 0, _usedNames.Length - 1)];

                Pig pigInstance = data.ToPig();
                visual.Setup(pigInstance, raceArea, this);
                visual.SetRunnerMode(dName, isPlayer);                          // Set up pig for race

                _runners.Add(visual);
                _speeds.Add(Random.Range(minBaseSpeed, maxBaseSpeed));
                _lengthsDone.Add(0);
                _direction.Add(1);
                visual.UpdateFacingDirection(true); // Face right at start
            }
        }

        void Update() {
            if (!_racing) return;
            
            if (counterText) counterText.text = $"Lengths : {CurrentLenght}/{totalLengths}";

            float startX = startLine.anchoredPosition.x;
            float endX = finishLine.anchoredPosition.x;

            for (int i = 0; i < _runners.Count; i++) {
                if (_lengthsDone[i] >= totalLengths) continue;

                RectTransform rt = _runners[i].GetComponent<RectTransform>();
                Vector2 pos = rt.anchoredPosition;

                pos.x += _speeds[i] * _direction[i] * Time.deltaTime;

                float targetX = _direction[i] > 0 ? endX : startX;
                bool reached = _direction[i] > 0 ? pos.x >= targetX : pos.x <= targetX;

                if (reached) {
                    pos.x = targetX;
                    _lengthsDone[i]++;

                    if (_lengthsDone[i] >= totalLengths) {
                        if (!_finishOrder.Contains(i)) _finishOrder.Add(i);
                        continue;
                    }

                    _direction[i] *= -1;
                    _runners[i].UpdateFacingDirection(_direction[i] == 1);
                }
                rt.anchoredPosition = pos;
            }

            if (_finishOrder.Count >= _runners.Count) OnRaceFinished();
        }

        public void StartRace() {
            if (_racing) return;

            _racing = true;
            if (startRaceButton) startRaceButton.SetActive(false);
        }

        private T Choice<T>(params T[] options) {
            return options[Random.Range(0, options.Length)];
        }

        private void OnRaceFinished() {
            _racing = false;
            int playerRank = _finishOrder.IndexOf(_playerIndex) + 1;
            
            List<UIPigVisual> rankedPigs = new();
            foreach (int runnerIdx in _finishOrder) rankedPigs.Add(_runners[runnerIdx]);

            ShowResults(rankedPigs, playerRank);
        }

        private void ShowResults(List<UIPigVisual> runners, int playerRank) {
            if (resultsScreen) resultsScreen.SetActive(true);

            bool isVictory = playerRank <= 3;

            if (resultsTitleImage) resultsTitleImage.sprite = isVictory ? victoryTitleSprite : defeatTitleSprite;  // Set title

            SetFx(confetti, isVictory);                                         // Activate particules
            SetFx(rain, !isVictory);

            foreach (var spot in podiumSpotPigs) foreach (Transform child in spot) Destroy(child.gameObject);

            for (int i = 0; i < podiumSpotPigs.Length; i++) {                   // Place pigs on podium
                if (i >= runners.Count) continue;

                GameObject go = Instantiate(pigPrefab, podiumSpotPigs[i]);
                UIPigVisual visual = go.GetComponent<UIPigVisual>();
                
                visual.Setup(runners[i].Data, podiumSpotPigs[i] as RectTransform, this);
                visual.SetPodiumMode();

                if (playerRank - 1 != i) continue;
                if (_endScreenCoroutine != null) StopCoroutine(_endScreenCoroutine);
                _endScreenCoroutine = StartCoroutine(EndScreenAnim(visual, isVictory));
            }
        }

        private IEnumerator EndScreenAnim(UIPigVisual playerVisual, bool victory) {
            Transform iconTransform = playerVisual.GetIconTransform();
            Vector3 startPos = iconTransform.localPosition;

            while (true) {
                if (victory) {                                                  // Jumping animation
                    float elapsed = 0;
                    while (elapsed < jumpUpTime) {
                        iconTransform.localPosition = Vector3.Lerp(startPos, startPos + Vector3.up * jumpOffsetY, elapsed / jumpUpTime);
                        elapsed += Time.deltaTime;
                        yield return null;
                    }
                    elapsed = 0;
                    while (elapsed < jumpUpTime) {
                        iconTransform.localPosition = Vector3.Lerp(startPos + Vector3.up * jumpOffsetY, startPos, elapsed / jumpUpTime);
                        elapsed += Time.deltaTime;
                        yield return null;
                    }
                    yield return new WaitForSeconds(top3LoopPause);
                } else {                                                        // Facing left & right animation
                    iconTransform.localScale = new Vector3(-1, 1, 1);
                    yield return new WaitForSeconds(0.6f);
                    iconTransform.localScale = new Vector3(1, 1, 1);
                    yield return new WaitForSeconds(0.6f);
                }
            }
        }

        public void Continue() {
            if (resultsScreen) resultsScreen.SetActive(false);
            if (startRaceButton) startRaceButton.SetActive(true);
            if (_endScreenCoroutine != null) StopCoroutine(_endScreenCoroutine);
            if (counterText) counterText.text = $"Lengths : {CurrentLenght}/{totalLengths}";
            Cleanup();
            CreatePigs();
        }

        private void Cleanup() {
            foreach (var r in _runners) if (r) Destroy(r.gameObject);
            _runners.Clear(); _speeds.Clear(); _lengthsDone.Clear();
            _direction.Clear(); _finishOrder.Clear();
        }

        private void SetFx(ParticleSystem ps, bool enable) {
            if (!ps) return;
            ps.gameObject.SetActive(enable);
            if (enable) ps.Play(); else ps.Stop();
        }
    }
}
