using UnityEngine.EventSystems;
using UnityEngine;

namespace UI {
    public class UIButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        [SerializeField] private bool hideByDefault = true;
        [SerializeField] private GameObject visualToToggle;

        private void Awake() {
            if (visualToToggle) visualToToggle.SetActive(!hideByDefault);
        }

        public void OnPointerEnter(PointerEventData eventData) => visualToToggle.SetActive(true);

        public void OnPointerExit(PointerEventData eventData) => visualToToggle.SetActive(false);
    
        private void OnDisable() {
            if (visualToToggle) visualToToggle.SetActive(false);
        }
    }
}
