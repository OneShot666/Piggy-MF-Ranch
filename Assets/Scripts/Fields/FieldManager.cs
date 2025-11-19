using UnityEngine;

// ReSharper disable UnusedMember.Global
namespace Fields {
    public class FieldManager : MonoBehaviour {
        public FieldPlot plotPrefab;
        public int width = 3;
        public int height = 3;
        public float spacing = 1.5f;

        private FieldPlot[,] _plots;

        private void Start() {
            _plots = new FieldPlot[width, height];

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    var plot = Instantiate(plotPrefab, transform);
                    plot.transform.localPosition = new Vector3(x * spacing, y * spacing, 0);

                    _plots[x, y] = plot;
                }
            }
        }

        public FieldPlot GetPlotAt(int gridX, int gridY) {
            if (gridX < 0 || gridX >= width) return null;
            if (gridY < 0 || gridY >= height) return null;
            return _plots[gridX, gridY];
        }
    }
}
