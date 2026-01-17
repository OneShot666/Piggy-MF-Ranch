using UnityEngine.UI;
using UnityEngine;
using System;
using Items;

namespace Fields {
    public class SeedButton : MonoBehaviour {
        [SerializeField] private Image iconImage;
        [SerializeField] private Text quantityText;

        private Button _button;

        public void SetSeed(ItemData data, int totalQuantity, Action onClick) {
            iconImage.sprite = data.icon;
            if (quantityText) quantityText.text = totalQuantity > 1 ? $"x{totalQuantity.ToString()}" : ""; // Display if more than 1

            _button = GetComponent<Button>();
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() => onClick?.Invoke());
        }
    }
}
