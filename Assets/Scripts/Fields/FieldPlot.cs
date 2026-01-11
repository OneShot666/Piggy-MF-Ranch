using UnityEngine.Events;
using UnityEngine;
using Items;

// ReSharper disable InconsistentNaming
namespace Fields {
    public class FieldPlot : MonoBehaviour {
        private enum PlotState { Empty, Growing, Ready }

        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;                 // Field image
        [SerializeField] private Sprite emptySprite;
        [SerializeField] private Sprite growingSprite;
        [SerializeField] private Sprite readySprite;

        [Header("Runtime Data")]
        [SerializeField] private PlotState state = PlotState.Empty;
        [SerializeField] private ItemData plantedSeed;

        [Header("Events")]
        public UnityEvent<ItemData> OnHarvest;                                  // Called when harvest field
        public UnityEvent<ItemData> OnPlant;                                    // Called when planting seeds

        private float growTimer;

        private void Update() {
            if (state != PlotState.Growing) return;

            growTimer += Time.deltaTime;
            if (growTimer >= plantedSeed.growTime) {
                state = PlotState.Ready;
                spriteRenderer.sprite = readySprite;
            }
        }

        public bool Plant(ItemData seed) {                                          // To call from Inventory system
            if (state != PlotState.Empty) return false;
            if (seed.type != ItemType.Seed) return false;

            plantedSeed = seed;
            growTimer = 0f;
            state = PlotState.Growing;

            spriteRenderer.sprite = growingSprite;
            OnPlant?.Invoke(seed);

            return true;
        }

        public bool Harvest() {
            if (state != PlotState.Ready) return false;

            ItemData crop = plantedSeed.cropProduced;

            // reset field
            plantedSeed = null;
            growTimer = 0f;
            state = PlotState.Empty;
            spriteRenderer.sprite = emptySprite;

            OnHarvest?.Invoke(crop);
            return true;
        }
    }
}
