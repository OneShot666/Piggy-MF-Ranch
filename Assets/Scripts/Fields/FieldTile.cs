using UnityEngine.UI;
using UnityEngine;

namespace Fields {
    public class FieldTile : MonoBehaviour {
        public Image groundImage;
        public Image cropImage;                                                 // Current status of plant above

        public void SetGround(Sprite s) => groundImage.sprite = s;

        public void SetCrop(Sprite s) {
            if (!cropImage) return;

            if (!s) cropImage.enabled = false;
            else {
                cropImage.enabled = true;
                cropImage.sprite = s;
            }
        }
    }
}
