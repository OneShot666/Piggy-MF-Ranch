using System.Collections.Generic;
using UnityEngine;
using Items;

namespace Fields {
    public class FieldPlot : MonoBehaviour {
        private enum PlotState { Empty, Growing, Ready }

        [Header("References")]
        [SerializeField] private Vector2Int size = new(3, 3);                   // Doesn't include borders
        [SerializeField] private FieldSprites fieldSprites;
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private Transform gridContainer;                       // Must have GridLayoutGroup
        [SerializeField] private List<Sprite> growthStages;

        [Header("Grow settings")]
        [SerializeField] private bool isWet;
        [SerializeField] private PlotState state = PlotState.Empty;
        [SerializeField] private ItemData plantedSeed;

        private readonly List<FieldTile> _tiles = new();
        private readonly List<FieldTile> _centerTiles = new();                  // Useful tile where plants grows

        private float _growTimer;

        private void Start() {
            GenerateField();
        }

        private void GenerateField() {
            if (!gridContainer || !tilePrefab || fieldSprites == null) return;

            int totalW = size.x + 2;                                            // Add borders to count
            int totalH = size.y + 2;

            foreach (Transform child in gridContainer) Destroy(child.gameObject);

            _tiles.Clear();
            _centerTiles.Clear();

            for (int y = totalH - 1; y >= 0; y--) {
                for (int x = 0; x < totalW; x++) {
                    GameObject go = Instantiate(tilePrefab, gridContainer);
                    FieldTile tile = go.GetComponent<FieldTile>();
                    
                    tile.SetGround(fieldSprites.GetSprite(x, y, totalW, totalH, isWet));
                    _tiles.Add(tile);

                    if (x > 0 && x < totalW - 1 && y > 0 && y < totalH - 1)     // If center tile
                        _centerTiles.Add(tile);
                }
            }
        }

        private void Update() {
            if (state != PlotState.Growing || !isWet) return;

            _growTimer += Time.deltaTime;
            UpdateGrowthVisuals();

            if (_growTimer >= plantedSeed.growTime) {
                state = PlotState.Ready;
                SetCropsSprite(plantedSeed.cropProduced.icon);                  // Display last status (ready to be harvest)
            }
        }

        private void UpdateGrowthVisuals() {
            if (!plantedSeed || growthStages.Count == 0) return;

            float progress = _growTimer / plantedSeed.growTime;                  // Get current status index
            int stageIndex = Mathf.FloorToInt(progress * growthStages.Count);
            stageIndex = Mathf.Clamp(stageIndex, 0, growthStages.Count - 1);

            SetCropsSprite(growthStages[stageIndex]);
        }

        private void SetCropsSprite(Sprite s) {
            foreach (var tile in _centerTiles) tile.SetCrop(s);
        }

        public void Water() {
            isWet = true;
            RefreshGroundVisuals();
        }

        private void RefreshGroundVisuals() {
            int totalW = size.x + 2;
            int totalH = size.y + 2;
            int i = 0;
            for (int y = totalH - 1; y >= 0; y--) {
                for (int x = 0; x < totalW; x++) {
                    _tiles[i].SetGround(fieldSprites.GetSprite(x, y, totalW, totalH, isWet));
                    i++;
                }
            }
        }

        public bool Plant(ItemData seed) {
            if (state != PlotState.Empty || seed.type != ItemType.Seed) return false;

            plantedSeed = seed;
            _growTimer = 0f;
            state = PlotState.Growing;
            UpdateGrowthVisuals();
            return true;
        }

        public void Harvest() {                                                 // Add result to inventory
            if (state != PlotState.Ready) return;

            plantedSeed = null;
            state = PlotState.Empty;
            isWet = false;                                                      // Field become dry after harvest
            SetCropsSprite(null);
            RefreshGroundVisuals();
        }
    }
}
