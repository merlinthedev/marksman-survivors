using System.Collections.Generic;
using EventBus;
using Inventory.Items;
using UnityEngine;
using Logger = Util.Logger;

namespace Interactable.NPC {
    public class Merchant : NPC, IInteractable {
        private List<Item> items = new();

        private void Start() {
            for (int i = 0; i < 5; i++) {
                Item item = new Item("item " + i);
                items.Add(item);
            }
        }

        public override void OnEndDialogue() {
            Destroy(gameObject);
        }

        public void OnInteract() {
            Logger.Log("interacting with merchant", Logger.Color.RED, this);
            EventBus<MerchantInteractEvent>.Raise(new MerchantInteractEvent(items));
        }
    }
}