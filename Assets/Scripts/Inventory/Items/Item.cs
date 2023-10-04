using Champions;
using UnityEngine;

namespace Inventory.Items {
    public abstract class Item : MonoBehaviour {
        private string name;
        [SerializeField] private int price;

        public abstract void OnEquip();
        public abstract void OnUnequip();

        #region Getters & Setters

        public string GetName() {
            return name;
        }

        public int GetPrice() {
            return price;
        }

        public override string ToString() {
            return "Name: " + name + ", price: " + price;
        }

        #endregion
    }
}