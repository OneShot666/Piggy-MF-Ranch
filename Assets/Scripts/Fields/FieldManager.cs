using UnityEngine.UI;
using UnityEngine;

namespace Fields {
    public class FieldManager : MonoBehaviour {
        [SerializeField] private FieldPlot plotPrefab;
        [SerializeField] private int width = 3;
        [SerializeField] private int height = 3;
        [SerializeField] private Vector2 fieldSize = new(300, 300);             // Size in pixels
        [SerializeField] private Vector2 spacing = new(20, 20);                 // Space between fields in pixels

        private FieldPlot[,] _plots;

        private void Start() {
            SetupGrid();
            GeneratePlots();
        }

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

        public FieldPlot GetPlotAt(int gridX, int gridY) {
            if (gridX < 0 || gridX >= width || gridY < 0 || gridY >= height) return null;
            return _plots[gridX, gridY];
        }
    }
}
