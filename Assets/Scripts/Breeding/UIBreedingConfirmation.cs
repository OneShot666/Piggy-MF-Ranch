using UnityEngine.UI;
using UnityEngine;
using System;
using Pigs;

namespace Breeding {
    public class UIBreedingConfirmation : MonoBehaviour {
        public Text infoText;
        public Button confirmButton;
        public Button cancelButton;

        public void SetupPopup(Pig p1, Pig p2, Action onConfirm) {
            infoText.text = $"REPRODUCTION\n\n" +
                $"Parent 1: {p1.Rarity} ({p1.Color})\n" +
                $"Parent 2: {p2.Rarity} ({p2.Color})\n\n" +
                $"Chances de mutation: 3%\n" +
                $"Vitesse moyenne estimée: {(p1.Speed + p2.Speed) / 2f:F1}";

            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => onConfirm());
            
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(() => gameObject.SetActive(false));
        }
    }
}
