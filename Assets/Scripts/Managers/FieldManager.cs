using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Fields;
using Save;

namespace Managers {
    public class FieldManager : MonoBehaviour {
        [SerializeField] private FieldPlot plotPrefab;
        [SerializeField] private int width = 3;
        [SerializeField] private int height = 3;
        [SerializeField] private Vector2 fieldSize = new(300, 300);             // Size in pixels
        [SerializeField] private Vector2 spacing = new(20, 20);                 // Space between fields in pixels

        private FieldPlot[,] _plots;

        private void Start() {
            SetupGrid();
            if (GameManager.Instance && GameManager.Instance.currentSave.cropfield.Count > 0)
                LoadSave(GameManager.Instance.currentSave.cropfield);
            else GeneratePlots();
        }

        public FieldPlot[,] GetAllPlots() => _plots;

        private void SetupGrid() {
            GridLayoutGroup grid = GetComponent<GridLayoutGroup>();             // Config own GridLayoutGroup composant
            if (!grid) grid = gameObject.AddComponent<GridLayoutGroup>();

            grid.cellSize = fieldSize;
            grid.spacing = spacing;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = width;
            grid.childAlignment = TextAnchor.MiddleCenter;
        }

        private void GeneratePlots() {
            _plots = new FieldPlot[width, height];

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    var plot = Instantiate(plotPrefab, transform);              // Auto-placed by GridLayoutGroup
                    plot.name = $"Plot_{x}_{y}";
                    plot.Init(fieldSize);                                       // Set size of plots
                    _plots[x, y] = plot;
                }
            }
        }

        private void LoadSave(List<FieldSaveData> savedData) {
            _plots = new FieldPlot[width, height];
            int dataIndex = 0;

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    var plot = Instantiate(plotPrefab, transform);
                    plot.Init(fieldSize);                                       // Init size
            
                    if (dataIndex < savedData.Count)                            // Apply saved data to plot
                        plot.LoadPlotData(savedData[dataIndex], GameManager.Instance.allPossibleItems);

                    _plots[x, y] = plot;
                    dataIndex++;
                }
            }
        }

        private void OnDisable() {                                              // Update save when change scene
            if (GameManager.Instance) {
                List<FieldSaveData> dataToSave = new List<FieldSaveData>();
                foreach(var plot in _plots) dataToSave.Add(plot.GetPlotSaveData());
                GameManager.Instance.SyncFields(dataToSave);
            }
        }
    }
}
