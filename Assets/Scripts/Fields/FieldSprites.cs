using UnityEngine;

namespace Fields {
    [System.Serializable]
    public class FieldSprites {
        [Header("Dry Tiles")]
        public Sprite topLeft;
        public Sprite top;
        public Sprite topRight;
        public Sprite left;
        public Sprite center;
        public Sprite right;
        public Sprite bottomLeft;
        public Sprite bottom;
        public Sprite bottomRight;

        [Header("Wet Tiles")]
        public Sprite topLeftW;
        public Sprite topW;
        public Sprite topRightW;
        public Sprite leftW;
        public Sprite centerW;
        public Sprite rightW;
        public Sprite bottomLeftW;
        public Sprite bottomW;
        public Sprite bottomRightW;

        public Sprite GetSprite(int x, int y, int w, int h, bool isWet) {
            if (x == 0 && y == h - 1) return isWet ? topLeftW : topLeft;
            if (x == w - 1 && y == h - 1) return isWet ? topRightW : topRight;
            if (x == 0 && y == 0) return isWet ? bottomLeftW : bottomLeft;
            if (x == w - 1 && y == 0) return isWet ? bottomRightW : bottomRight;
            if (y == h - 1) return isWet ? topW : top;
            if (y == 0) return isWet ? bottomW : bottom;
            if (x == 0) return isWet ? leftW : left;
            if (x == w - 1) return isWet ? rightW : right;
            return isWet ? centerW : center;
        }
    }
}
