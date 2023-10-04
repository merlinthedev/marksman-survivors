using System.Collections.Generic;
using EventBus;
using Inventory.Items;
using UnityEngine;
using Logger = Util.Logger;

namespace Interactable.NPC {
    public class Merchant : NPC, IInteractable {
        [SerializeField] private List<Item> items = new();


        public override void OnEndDialogue() {
            Destroy(gameObject);
        }

        public void OnInteract() {
            Logger.Log("interacting with merchant", Logger.Color.RED, this);
            EventBus<MerchantInteractEvent>.Raise(new MerchantInteractEvent(items));
        }
    }
}