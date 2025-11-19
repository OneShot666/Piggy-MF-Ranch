using System.Collections.Generic;
using UnityEngine;
using Items;

// ReSharper disable UnusedMember.Global
namespace Market {
    public class MarketManager : MonoBehaviour {
        [Header("Market Settings")]
        public MarketItemPool itemPool;
        public int stallSize = 6;

        [Header("Generated Offers (Runtime)")]
        public List<MarketOffer> currentOffers = new();

        private void Start() {
            GenerateMarket();
        }

        private void GenerateMarket() {
            currentOffers.Clear();

            if (!itemPool || itemPool.possibleItems.Length == 0) {
                Debug.LogWarning("Market item pool is empty!");
                return;
            }

            for (int i = 0; i < stallSize; i++) {
                ItemData item = itemPool.possibleItems[
                    Random.Range(0, itemPool.possibleItems.Length)
                ];

                MarketOffer offer = new MarketOffer { item = item,
                    quantity = Random.Range(1, 11)                              // Quantity between 1 and 10
                };

                // Prix de base = prix unitaire * quantité
                offer.basePrice = item.buyPrice * offer.quantity;

                // 30% de chance d'avoir une remise
                if (Random.value < 0.3f) {
                    float discountPercent = Random.Range(0.10f, 0.50f);
                    offer.discount = discountPercent;
                } else {
                    offer.discount = 0f;
                }

                currentOffers.Add(offer);
            }
        }

        public bool BuyOffer(int index, Inventory inventory) {
            if (index < 0 || index >= currentOffers.Count) return false;

            MarketOffer offer = currentOffers[index];

            if (inventory.money < offer.FinalPrice) return false;

            inventory.money -= offer.FinalPrice;
            inventory.AddItem(offer.item, offer.quantity);

            currentOffers.RemoveAt(index);

            return true;
        }
    }
}
