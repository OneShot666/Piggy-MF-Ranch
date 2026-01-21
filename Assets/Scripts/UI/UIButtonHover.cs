using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

namespace UI {
    [RequireComponent(typeof(Image))]
    public class UIButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        [SerializeField] private bool hideByDefault = true;
        [SerializeField] private GameObject visualToToggle;

        private void Awake() {
            Image image = GetComponent<Image>();
            image.raycastTarget = true;

            if (visualToToggle) visualToToggle.SetActive(!hideByDefault);
        }

        public void OnPointerEnter(PointerEventData eventData) => visualToToggle.SetActive(true);

        public void OnPointerExit(PointerEventData eventData) => visualToToggle.SetActive(false);
    
        private void OnDisable() {
            if (visualToToggle) visualToToggle.SetActive(false);
        }
    }
}
