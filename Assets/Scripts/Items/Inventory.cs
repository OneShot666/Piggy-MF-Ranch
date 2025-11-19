using System.Collections.Generic;
using UnityEngine;

// ReSharper disable UnusedMember.Global
namespace Items {
    public class Inventory : MonoBehaviour {
        public List<ItemInstance> items = new();
        public int money;

        public void AddItem(ItemData data, int amount = 1) {
            if (data.stackable) {
                var existing = items.Find(i => i.data == data);
                if (existing != null) {
                    existing.quantity += amount;
                    return;
                }
            }
            items.Add(new ItemInstance(data, amount));
        }

        public void RemoveItem(ItemData data, int amount = 1) {
            var slot = items.Find(i => i.data == data);
            if (slot == null) return;

            slot.quantity -= amount;
            if (slot.quantity <= 0) items.Remove(slot);
        }
    }
}
