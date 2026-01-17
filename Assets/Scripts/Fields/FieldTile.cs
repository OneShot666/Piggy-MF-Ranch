using UnityEngine.UI;
using UnityEngine;
using Items;
using Save;

namespace Fields {
    public class FieldTile : MonoBehaviour {
        public enum TileState { Empty, Growing, Ready }

        [Header("References")]
        [SerializeField] private Image groundImage;
        [SerializeField] private Image cropImage;                               // Current status of plant above

        [Header("Grow settings")]
        [SerializeField] private bool isWet;
        [SerializeField] private TileState currentState = TileState.Empty;

        private FieldPlot _parentPlot;
        private ItemData _plantedSeed;
        private float _growTimer;
        
        public bool IsWet => isWet;

        public TileState CurrentState => currentState;

        private void Update() {
            if (currentState != TileState.Growing || !isWet) return;

            _growTimer += Time.deltaTime;
            UpdateVisuals();

            if (_plantedSeed && _growTimer >= _plantedSeed.growTime) {          // If plant is ready to be harvest
                currentState = TileState.Ready;
                SetCrop(_plantedSeed.cropProduced.icon);
            }
        }

        private void UpdateVisuals() {                                          // Check tile image
            if (!_plantedSeed || _parentPlot.GrowthStages.Count == 0) return;

            float progress = _growTimer / _plantedSeed.growTime;
            int stageIndex = Mathf.FloorToInt(progress * _parentPlot.GrowthStages.Count);
            stageIndex = Mathf.Clamp(stageIndex, 0, _parentPlot.GrowthStages.Count - 1);

            SetCrop(_parentPlot.GrowthStages[stageIndex]);
        }

        public string GetPlantedSeedName() => _plantedSeed ? _plantedSeed.itemName : null;

        public float GetTimer() => _growTimer;

        public void SetTileParent(FieldPlot plot) => _parentPlot = plot;        // FieldTile are always in FieldPlot (normally)

        public void SetGround(Sprite s, bool waterStatus) {                     // Change bg image
            isWet = waterStatus;
            groundImage.sprite = s;
        }

        public void SetCrop(Sprite s) {                                         // Change seed/plant image
            if (!cropImage) return;

            cropImage.enabled = s;
            cropImage.sprite = s;
        }

        public void SetWetness(bool wet) => isWet = wet;                        // If tile is watered or not

        public void Water(Sprite wetSprite) {                                   // Water tile
            isWet = true;
            groundImage.sprite = wetSprite;
        }

        public void Plant(ItemData seed) {                                      // Plant seed in tile
            if (currentState != TileState.Empty) return;

            _plantedSeed = seed;
            _growTimer = 0;
            currentState = TileState.Growing;
            
            UpdateVisuals();
        }

        public ItemData Harvest() {                                             // Harvest fruit/vegetable from tile
            if (currentState != TileState.Ready) return null;

            ItemData produce = _plantedSeed.cropProduced;
            _plantedSeed = null;
            currentState = TileState.Empty;
            isWet = false;
            SetCrop(null);

            return produce;
        }

        public TileSaveData GetSaveData() {                                     // Create save object with tile data
            return new TileSaveData { state = (int)currentState, isWet = isWet,
                seedName = currentState != TileState.Empty && _plantedSeed ? _plantedSeed.name : "",
                growTimer = _growTimer
            };
        }

        public void LoadData(TileSaveData data, ItemData seedRef) {             // Restore tile status
            currentState = (TileState) data.state;
            isWet = data.isWet;
            _growTimer = data.growTimer;
            _plantedSeed = seedRef;

            if (currentState == TileState.Ready && _plantedSeed)
                SetCrop(_plantedSeed.cropProduced.icon);                        // Instant visual update
            else if (currentState == TileState.Growing) UpdateVisuals();
            else SetCrop(null);
        }
    }
}
