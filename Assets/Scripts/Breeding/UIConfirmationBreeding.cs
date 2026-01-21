using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using System;
using Pigs;
using UI;

namespace Breeding {
    public class UIConfirmationBreeding : MonoBehaviour {
        [Header("References")]
        [SerializeField] private BreedingSystem breedingSystem;
        [SerializeField] private Image parent1Image;
        [SerializeField] private Text parent1Text;
        [SerializeField] private Image parent2Image;
        [SerializeField] private Text parent2Text;
        [SerializeField] private Image pigletImage;
        [SerializeField] private Text pigletText;
        [SerializeField] private Text bgButtonText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        private UIButtonHover _hoverScript;
        private Image _cancelBgImage;

        private void Start() {
            if (!breedingSystem) breedingSystem = FindFirstObjectByType<BreedingSystem>();
            _hoverScript = GetComponentInChildren<UIButtonHover>();
            _cancelBgImage = cancelButton ? cancelButton.GetComponent<Image>() : null;
            AddHoverEvent();
        }

        public void SetupPopup(Pig p1, Pig p2, Action onConfirm, bool isFull=false) {
            if (p1 == null || p2 == null) return;

            if (parent1Image) parent1Image.sprite = p1.Icon;
            if (parent1Text) {
                parent1Text.text = "";                                  // Reset text
                parent1Text.color = p1.GetColor();
                parent1Text.text += $"Color :  {p1.SkinColor}\n";
                parent1Text.text += $"Rarity : {p1.Rarity.ToString()}\n";
                parent2Text.text += $"Speed :  {p1.Speed}\n";
                if (p1.ActivePower != null) parent1Text.text += $"Power :  {p1.ActivePower.ToString()}\n";
                if (p1.PassivePower != null) parent1Text.text += $"Passive : {p1.PassivePower.ToString()}\n";
                parent1Text.text += $"Mutation chance :  : {p1.MutationBonus}%\n";
                parent1Text.text += $"Generation : {p1.Generation}";
            }

            if (parent2Image) parent2Image.sprite = p2.Icon;
            if (parent2Text) {
                parent2Text.text = "";                                  // Reset text
                parent2Text.color = p2.GetColor();
                parent2Text.text += $"Color :  {p2.SkinColor}\n";
                parent2Text.text += $"Rarity : {p2.Rarity.ToString()}\n";
                parent2Text.text += $"Speed :  {p2.Speed}\n";
                if (p2.ActivePower != null) parent2Text.text += $"Power :  {p2.ActivePower.ToString()}\n";
                if (p2.PassivePower != null) parent2Text.text += $"Passive : {p2.PassivePower.ToString()}\n";
                parent2Text.text += $"Mutation chance :  : {p2.MutationBonus}%\n";
                parent2Text.text += $"Generation : {p2.Generation}";
            }

            if (!breedingSystem) return;
            Pig piglet = breedingSystem.Breed(p1, p2);
            if (pigletImage) pigletImage.sprite = piglet.Icon;
            if (pigletText) {
                pigletText.text = "";                                   // Reset text
                pigletText.color = piglet.GetColor();
                pigletText.text += $"Possible color :  {piglet.SkinColor}\n";
                pigletText.text += $"Possible rarity : {piglet.Rarity.ToString()}\n";
                pigletText.text += $"Possible speed :  {piglet.Speed}\n";
                if (piglet.ActivePower != null) pigletText.text += $"Possible power :  {piglet.ActivePower.ToString()}\n";
                if (piglet.PassivePower != null) pigletText.text += $"Possible passive : {piglet.PassivePower.ToString()}\n";
                pigletText.text += $"Possible mutation chance :  : {piglet.MutationBonus}%\n";
                pigletText.text += $"Generation : {piglet.Generation}";
            }

            if (bgButtonText) bgButtonText.text = isFull ? "Enclosure full" : "Confirm";

            if (_hoverScript) _hoverScript.enabled = !isFull;

            if (confirmButton) {
                confirmButton.enabled = !isFull;
                confirmButton.onClick.RemoveAllListeners();
                confirmButton.onClick.AddListener(() => onConfirm()); // Confirm button trigger given action
            }

            if (cancelButton) {
                cancelButton.onClick.RemoveAllListeners();
                cancelButton.onClick.AddListener(() => { gameObject.SetActive(false); });
            }
        }

        public void OpenWindow() {
            gameObject.SetActive(true);
            if (_cancelBgImage) _cancelBgImage.enabled = false;
        }

        public void CloseWindow() => gameObject.SetActive(false);

        private void AddHoverEvent() {
            if (!cancelButton) return;

            Button btn = cancelButton ? cancelButton.GetComponent<Button>() : null;
            if (!btn || !_cancelBgImage) return;

            _cancelBgImage.enabled = false;                                     // Hide by default

            EventTrigger trigger = btn.gameObject.GetComponent<EventTrigger>();
            if (!trigger) trigger = btn.gameObject.AddComponent<EventTrigger>();

            // Event : PointerEnter
            EventTrigger.Entry entryHover = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            entryHover.callback.AddListener(_ => { _cancelBgImage.enabled = true; });
            trigger.triggers.Add(entryHover);

            // Event : PointerExit
            EventTrigger.Entry entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            entryExit.callback.AddListener(_ => { _cancelBgImage.enabled = false; });
            trigger.triggers.Add(entryExit);
        }
    }
}
