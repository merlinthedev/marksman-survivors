using _Scripts.EventBus;
using _Scripts.Inventory.Items;
using System.Collections.Generic;
using UnityEngine;
using Logger = _Scripts.Util.Logger;

namespace _Scripts.Interactable.NPC {
    public class Merchant : NPC, IInteractable {
        [SerializeField] private List<Item> items = new();


        public override void OnEndDialogue() {
            Destroy(gameObject);
        }

        public void OnInteract() {
            Logger.Log("interacting with merchant", Logger.Color.RED, this);
            EventBus<MerchantInteractEvent>.Raise(new MerchantInteractEvent(items, this));
        }
    }
}