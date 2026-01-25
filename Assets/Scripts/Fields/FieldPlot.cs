using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using Items;
using Save;

namespace Fields {
    public class FieldPlot : MonoBehaviour, IPointerClickHandler {
        [Header("References")]
        [SerializeField] private Vector2Int size = new(3, 3);                   // Doesn't include borders
        [SerializeField] private FieldSprites fieldSprites;
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private Transform gridContainer;                       // Must have GridLayoutGroup
        [SerializeField] private List<Sprite> growthStages;

        private readonly List<FieldTile> _tiles = new();
        private readonly List<FieldTile> _centerTiles = new();                  // Useful tile where plants grows
        
        public List<Sprite> GrowthStages => growthStages;

        public void Init(Vector2 pixelSize) {
            RectTransform rt = GetComponent<RectTransform>();
            rt.sizeDelta = pixelSize;

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

                    if (tile) {
                        tile.SetTileParent(this);
                        Sprite s = fieldSprites.GetSprite(x, y, totalW, totalH, tile.IsWet);
                        tile.SetGround(s, tile.IsWet);
                        _tiles.Add(tile);
                    }

                    if (x > 0 && x < totalW - 1 && y > 0 && y < totalH - 1)     // If center tile
                        _centerTiles.Add(tile);
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData) {                // When interact with field
            if (HasSomethingToHarvest()) HarvestAll();                          // Harvest first
            else if (IsAnyTileDry()) WaterAll();                                // Then water field
            else SeedSelector.Instance.Open(this);                          // And plant seeds
        }

        private bool HasSomethingToHarvest() {
            foreach (var tile in _centerTiles) 
                if (tile.CurrentState == FieldTile.TileState.Ready) return true;
            return false;
        }

        private bool IsAnyTileDry() {
            foreach (var tile in _centerTiles)
                if (tile.CurrentState == FieldTile.TileState.Empty && !tile.IsWet) return true;
            return false;
        }

        private void WaterAll() {
            int totalW = size.x + 2;
            int totalH = size.y + 2;
            
            int i = 0;
            for (int y = totalH - 1; y >= 0; y--) {
                for (int x = 0; x < totalW; x++) {
                    Sprite s = fieldSprites.GetSprite(x, y, totalW, totalH, true);
                    _tiles[i].Water(s);
                    i++;
                }
            }
        }

        public void PlantMax(ItemData seed) {
            InventoryManager inv = InventoryManager.Instance;
            if (!inv) return;

            int availableSeeds = inv.GetTotalQuantity(seed);
            int plantedCount = 0;

            foreach (var tile in _centerTiles) {
                if (availableSeeds <= 0) break;

                if (tile.CurrentState == FieldTile.TileState.Empty && tile.IsWet) {
                    tile.Plant(seed);                                           // Each tile manage self
                    availableSeeds--;
                    plantedCount++;
                }
            }

            if (plantedCount > 0) inv.RemoveItem(seed, plantedCount);
        }

        private void HarvestAll() {                                              // Add result to inventory
            bool hasChanged = false;

            foreach (var tile in _centerTiles) {
                if (tile.CurrentState == FieldTile.TileState.Ready) {
                    ItemData result = tile.Harvest();
                    if (result) {
                        InventoryManager.Instance.AddItem(result);
                        hasChanged = true;
                    }
                }
            }

            if (hasChanged) CheckIfFieldShouldDry();                            // Update visuals once harvest -> dry
        }

        private void CheckIfFieldShouldDry() {
            bool stillHasCrops = false;

            foreach (var tile in _centerTiles) {                                // Check if there is still a seed in crop
                if (tile.CurrentState != FieldTile.TileState.Empty) {
                    stillHasCrops = true;
                    break;
                }
            }

            if (!stillHasCrops) foreach (var tile in _tiles) tile.SetWetness(false);    // If crop is empty, make all tile dry

            RefreshGroundVisuals();
        }

        private void RefreshGroundVisuals() {
            int totalW = size.x + 2;
            int totalH = size.y + 2;
            int i = 0;
            for (int y = totalH - 1; y >= 0; y--) {
                for (int x = 0; x < totalW; x++) {
                    Sprite s = fieldSprites.GetSprite(x, y, totalW, totalH, _tiles[i].IsWet);
                    _tiles[i].SetGround(s, _tiles[i].IsWet);
                    i++;
                }
            }
        }

        public FieldSaveData GetPlotSaveData() {
            FieldSaveData plotData = new FieldSaveData();
            foreach (var tile in _centerTiles) plotData.tiles.Add(tile.GetSaveData());
            return plotData;
        }

        public TileSaveData GetTileSaveData(int index=0) {
            return new TileSaveData { state = (int)_centerTiles[index].CurrentState,
                isWet = _centerTiles[index].IsWet, seedName = _centerTiles[index].GetPlantedSeedName(),
                growTimer = _centerTiles[index].GetTimer()
            };
        }

        public void LoadPlotData(FieldSaveData data, List<ItemData> allItems) {
            if (data == null || data.tiles == null || data.tiles.Count == 0) return;
            bool isWet = false;

            for (int i = 0; i < _centerTiles.Count; i++) {
                if (i >= data.tiles.Count) break;

                var tData = data.tiles[i];
                ItemData seed = allItems.Find(s => s.name == tData.seedName);   // Find seed by name
                _centerTiles[i].LoadData(tData, seed);
                if (_centerTiles[i].IsWet) isWet = true;
            }
            
            if (isWet) WaterAll();

            RefreshGroundVisuals();
        }
    }
}
