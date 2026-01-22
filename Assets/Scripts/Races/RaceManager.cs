using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using Breeding;
using Pigs;

// ReSharper disable IteratorNeverReturns
// ReSharper disable Unity.PerformanceCriticalCodeInvocation
namespace Races {
    public class RaceManager : MonoBehaviour {
        [Header("Screens references")]
        [SerializeField] private GameObject raceScreen;
        [SerializeField] private GameObject resultsScreen;       // Le panel unique
        [SerializeField] private Image resultsTitleImage;        // L'objet Image du titre
        [SerializeField] private Sprite victoryTitleSprite;      // Le sprite "Victoire"
        [SerializeField] private Sprite defeatTitleSprite;       // Le sprite "Défaite"

        [Header("Race Layout")]
        [SerializeField] private RectTransform raceArea;
        [SerializeField] private List<RectTransform> raceSlots = new();
        [SerializeField] private RectTransform startLine;
        [SerializeField] private RectTransform finishLine;

        [Header("Race Settings")]
        [SerializeField] private GameObject pigPrefab; // Un seul prefab maintenant
        [SerializeField] private List<PigData> possiblePigsData; // Données pour varier les visuels
        [SerializeField] private float minBaseSpeed = 150f;
        [SerializeField] private float maxBaseSpeed = 250f;
        [SerializeField] private int totalLengths = 3;

        [Header("Player settings")]
        [SerializeField] private string playerName = "Player_Pig";
        [SerializeField] private PigData playerPigData; // Pour que le joueur ait un visuel précis
        
        [Header("UI & FX")]
        [SerializeField] private GameObject startRaceButton;
        [SerializeField] private ParticleSystem confetti;
        [SerializeField] private ParticleSystem rain;
        [SerializeField] private Transform[] podiumSpotPigs = new Transform[6];

        [Header("Animation settings")]
        [SerializeField] private float jumpOffsetY = 50f;
        [SerializeField] private float jumpUpTime = 0.15f;
        [SerializeField] private float top3LoopPause = 0.3f;

        private readonly string[] _pigsNames = { "Julien", "Jonathan", "Gabriel", "Nathan", "Rayane" };
        private readonly List<UIPigVisual> _runners = new();
        private readonly List<float> _speeds = new();
        private readonly List<int> _lengthsDone = new();
        private readonly List<int> _direction = new();
        private readonly List<int> _finishOrder = new();
        
        private int _playerIndex;
        private bool _racing;
        private Coroutine _endScreenCoroutine;

        private void Awake() {
            ShowScreen(raceScreen);
            SetFx(confetti, false);
            SetFx(rain, false);
        }

        private void ShowScreen(GameObject screenToShow) {
            if (raceScreen) raceScreen.SetActive(raceScreen == screenToShow);
            if (resultsScreen) resultsScreen.SetActive(resultsScreen == screenToShow);
        }

        public void StartRace() {
            if (_racing) return;
            if (startRaceButton) startRaceButton.SetActive(false);
            LaunchRace();
            _racing = true;
        }

        private void LaunchRace() {
            Cleanup();
            _playerIndex = Random.Range(0, raceSlots.Count); 

            int aiNameCursor = 0;

            for (int i = 0; i < raceSlots.Count; i++) {
                GameObject inst = Instantiate(pigPrefab, raceArea);
                UIPigVisual visual = inst.GetComponent<UIPigVisual>();
                RectTransform rt = inst.GetComponent<RectTransform>();

                if (i < raceSlots.Count) rt.anchoredPosition = raceSlots[i].anchoredPosition;

                bool isPlayer = i == _playerIndex;
                
                // On choisit les données (SO)
                PigData data = isPlayer ? playerPigData : possiblePigsData[Random.Range(0, possiblePigsData.Count)];
                string dName = isPlayer ? playerName : _pigsNames[Mathf.Clamp(aiNameCursor++, 0, _pigsNames.Length - 1)];

                // On initialise le cochon avec ses données
                Pig pigInstance = data.ToPig();
                visual.Setup(pigInstance, raceArea, this);
                
                // IMPORTANT : On active le mode course pour stopper le mouvement aléatoire de UIPigVisual
                visual.SetRunnerMode(dName, isPlayer);

                _runners.Add(visual);
                _speeds.Add(Random.Range(minBaseSpeed, maxBaseSpeed));
                _lengthsDone.Add(0);
                _direction.Add(1);
                visual.UpdateFacingDirection(true); // Regarde à droite au départ
            }
        }

        void Update() {
            if (!_racing) return;

            float startX = startLine.anchoredPosition.x;
            float endX = finishLine.anchoredPosition.x;

            for (int i = 0; i < _runners.Count; i++) {
                if (_lengthsDone[i] >= totalLengths) continue;

                RectTransform rt = _runners[i].GetComponent<RectTransform>();
                Vector2 pos = rt.anchoredPosition;

                pos.x += _speeds[i] * _direction[i] * Time.deltaTime;

                float targetX = (_direction[i] > 0) ? endX : startX;
                bool reached = (_direction[i] > 0) ? pos.x >= targetX : pos.x <= targetX;

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

        private void OnRaceFinished() {
            _racing = false;
            int playerRank = _finishOrder.IndexOf(_playerIndex) + 1;
            
            List<Sprite> rankedSprites = new();
            foreach (int runnerIdx in _finishOrder) {
                // On utilise directement l'icône de la donnée Pig
                rankedSprites.Add(_runners[runnerIdx].Data.Icon);
            }

            ShowResults(rankedSprites, playerRank);
        }

        private void ShowResults(List<Sprite> sprites, int rank) {
            bool isVictory = rank <= 3;
            ShowScreen(resultsScreen);

            // 1. On change le titre
            if (resultsTitleImage) {
                resultsTitleImage.sprite = isVictory ? victoryTitleSprite : defeatTitleSprite;
            }

            // 2. On active les bons effets
            SetFx(confetti, isVictory);
            SetFx(rain, !isVictory);

            // 3. Remplissage du podium (le reste du code est identique)
            for (int i = 0; i < podiumSpotPigs.Length; i++) {
                if (i >= sprites.Count) continue;
                Image slotImg = podiumSpotPigs[i].GetComponentInChildren<Image>();
                if (slotImg) {
                    slotImg.sprite = sprites[i];
                    slotImg.enabled = sprites[i];
                }
            }

            if (_endScreenCoroutine != null) StopCoroutine(_endScreenCoroutine);
            _endScreenCoroutine = StartCoroutine(EndScreenAnim(podiumSpotPigs[rank - 1], isVictory));
        }

        private IEnumerator EndScreenAnim(Transform playerSpot, bool victory) {
            Vector3 startPos = playerSpot.localPosition;
            while (true) {
                if (victory) {
                    float elapsed = 0;
                    while (elapsed < jumpUpTime) {
                        playerSpot.localPosition = Vector3.Lerp(startPos, startPos + Vector3.up * jumpOffsetY, elapsed / jumpUpTime);
                        elapsed += Time.deltaTime;
                        yield return null;
                    }
                    elapsed = 0;
                    while (elapsed < jumpUpTime) {
                        playerSpot.localPosition = Vector3.Lerp(startPos + Vector3.up * jumpOffsetY, startPos, elapsed / jumpUpTime);
                        elapsed += Time.deltaTime;
                        yield return null;
                    }
                    yield return new WaitForSeconds(top3LoopPause);
                } else {
                    playerSpot.localEulerAngles = new Vector3(0, 180, 0);
                    yield return new WaitForSeconds(0.6f);
                    playerSpot.localEulerAngles = Vector3.zero;
                    yield return new WaitForSeconds(0.6f);
                }
            }
        }

        public void Continue() {
            if (resultsScreen) resultsScreen.SetActive(false);
            if (_endScreenCoroutine != null) StopCoroutine(_endScreenCoroutine);
            Cleanup();
            ShowScreen(raceScreen);
            if (startRaceButton) startRaceButton.SetActive(true);
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
