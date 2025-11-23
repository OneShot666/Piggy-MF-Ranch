using UnityEngine.UI;
using UnityEngine;
using Market;

namespace Items {
    public class MarketUIItem : MonoBehaviour {
        private int _index;
        private MarketManager _market;
        private InventoryManager _inventory;

        public void Init(int index, MarketManager market, InventoryManager inventory) {
            _index = index;
            _market = market;
            _inventory = inventory;

            GetComponent<Button>().onClick.AddListener(OnClick);                // Add click listener
        }

        private void OnClick() {
            if (!_market || !_inventory) return;

            bool success = _market.BuyOffer(_index, _inventory);
            _inventory.UpdateMoneyText();

            if (!success) Debug.Log("Not enough money!");
        }
    }
}
