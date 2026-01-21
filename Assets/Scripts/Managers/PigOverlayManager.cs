using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;
using Pigs;

namespace Managers {
    public class UIPigOverlayManager : MonoBehaviour {
        [Header("References")]
        [SerializeField] private RectTransform overlayRoot;
        [SerializeField] private Image pigImage;
        [SerializeField] private Text summaryText;
        [SerializeField] private Text statsText;

        [Header("Settings")]
        [SerializeField] private Vector2 mouseOffset = new(100, 100);

        private Canvas _canvas;

        public static UIPigOverlayManager Instance;

        void Awake() {
            if (!Instance) Instance = this;
            _canvas = GetComponentInParent<Canvas>();
            Hide();                                                             // Hide by default
        }

        void Update() {
            if (overlayRoot.gameObject.activeSelf) FollowMouse();
        }

        private void FollowMouse() {
            if (Mouse.current == null) return;

            Vector2 mousePos = Mouse.current.position.ReadValue();              // Get mouse position
            Camera cam = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, 
                mousePos, cam, out var localPos);

            overlayRoot.anchoredPosition = localPos + mouseOffset;
        }

        public void Show(Pig pig) {
            overlayRoot.gameObject.SetActive(true);
            
            if (pigImage) pigImage.sprite = pig.Icon;

            if (summaryText) {
                summaryText.text = "";
                summaryText.color = pig.GetColor();                             // Apply pig color to text color

                summaryText.text += $"Color : {pig.SkinColor}\n";
                summaryText.text += $"Rarity : {pig.Rarity.ToString()}\n";
                summaryText.text += $"Power : {(pig.ActivePower != null ? pig.ActivePower.ToString() : "None")}\n";
                summaryText.text += $"Passive : {(pig.PassivePower != null ? pig.PassivePower.ToString() : "None")}";
            }

            if (statsText) {
                statsText.text = "";

                statsText.text += $"Hunger : {pig.Hunger}/{pig.HungerMax}\n";
                statsText.text += $"Happiness : {pig.Happiness}/{pig.HappinessMax}\n";
                statsText.text += $"Cleanliness : {pig.Cleanliness}/{pig.CleanlinessMax}\n";
                statsText.text += $"Endurance : {pig.Endurance}/{pig.EnduranceMax}\n";
                statsText.text += $"Speed : {pig.Speed}\n";
                statsText.text += $"Mutation chance :  : {pig.MutationBonus}%\n";
                statsText.text += $"Generation : {pig.Generation}";
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(overlayRoot);           // Forced UI update
        }

        public void Hide() {
            if (overlayRoot) overlayRoot.gameObject.SetActive(false);
        }
    }
}
