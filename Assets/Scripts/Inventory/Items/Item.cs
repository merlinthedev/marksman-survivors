using System;

namespace Inventory.Items {
    public class Item {
        private string name = "";
        private int price;

        public Item(string name) {
            this.name = name;
        }

        public int GetPrice() {
            return price;
        }

        public void SetPrice(int price) {
            this.price = price;
        }

        public override string ToString() {
            return name;
        }
    }
}