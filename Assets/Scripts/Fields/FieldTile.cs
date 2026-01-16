using UnityEngine.UI;
using UnityEngine;
using Items;

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

            if (_plantedSeed && _growTimer >= _plantedSeed.growTime) {
                currentState = TileState.Ready;
                SetCrop(_plantedSeed.cropProduced.icon);
            }
        }

        private void UpdateVisuals() {
            if (!_plantedSeed || _parentPlot.GrowthStages.Count == 0) return;

            float progress = _growTimer / _plantedSeed.growTime;
            int stageIndex = Mathf.FloorToInt(progress * _parentPlot.GrowthStages.Count);
            stageIndex = Mathf.Clamp(stageIndex, 0, _parentPlot.GrowthStages.Count - 1);

            SetCrop(_parentPlot.GrowthStages[stageIndex]);
        }

        public void SetTileParent(FieldPlot plot) => _parentPlot = plot;

        public void SetGround(Sprite s, bool waterStatus) {
            isWet = waterStatus;
            groundImage.sprite = s;
        }

        public void SetCrop(Sprite s) {
            if (!cropImage) return;

            cropImage.enabled = s;
            cropImage.sprite = s;
        }

        public void Water(Sprite wetSprite) {
            isWet = true;
            groundImage.sprite = wetSprite;
        }

        public void Plant(ItemData seed) {
            if (currentState != TileState.Empty) return;

            _plantedSeed = seed;
            _growTimer = 0;
            currentState = TileState.Growing;
            
            UpdateVisuals();
        }

        public ItemData Harvest() {
            if (currentState != TileState.Ready) return null;

            ItemData produce = _plantedSeed.cropProduced;
            _plantedSeed = null;
            currentState = TileState.Empty;
            isWet = false;
            SetCrop(null);

            return produce;
        }
    }
}
