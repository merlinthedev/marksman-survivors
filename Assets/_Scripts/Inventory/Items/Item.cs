using UnityEngine;

namespace _Scripts.Inventory.Items {
    public abstract class Item : MonoBehaviour {
        [SerializeField] private string name;
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