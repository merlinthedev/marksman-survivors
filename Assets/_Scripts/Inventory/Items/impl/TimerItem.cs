using System.Collections;
using UnityEngine;

namespace _Scripts.Inventory.Items.impl {
    public class TimerItem : Item {
        [SerializeField] private float interval;
        [SerializeField] private int amountOfIntervals;
        
        
        public override void OnEquip() {
            throw new System.NotImplementedException();
        }

        public override void OnUnequip() {
            throw new System.NotImplementedException();
        }

        private IEnumerator timer() {
            yield break;
        }
    }
}