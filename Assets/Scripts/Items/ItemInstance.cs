// ReSharper disable NotAccessedField.Global
namespace Items {
    [System.Serializable]
    public class ItemInstance {
        public ItemData data;
        public int quantity;

        public ItemInstance(ItemData data, int quantity = 1) {
            this.data = data;
            this.quantity = quantity;
        }
    }
}
