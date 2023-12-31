﻿using _Scripts.EventBus;
using _Scripts.Inventory.Items;
using _Scripts.Util;
using System.Collections.Generic;

namespace _Scripts.Inventory {
    public class Inventory {
        private int gold;
        private int killCount;

        private List<Item> items = new();

        public Inventory() {
            gold = 0;
            killCount = 0;

            // Subscribe to event
            EventBus<EnemyKilledEvent>.Subscribe(OnEnemyKilled);
            EventBus<MerchantItemBuyRequestEvent>.Subscribe(OnMerchantItemBought);
            EventBus<AddGoldEvent>.Subscribe(AddGoldListener);
        }

        ~Inventory() {
            // Unsubscribe from event
            EventBus<EnemyKilledEvent>.Unsubscribe(OnEnemyKilled);
            EventBus<MerchantItemBuyRequestEvent>.Unsubscribe(OnMerchantItemBought);
            EventBus<AddGoldEvent>.Unsubscribe(AddGoldListener);
        }

        private void OnEnemyKilled(EnemyKilledEvent enemyKilledEvent) {
            AddKills(1);
        }

        private void OnMerchantItemBought(MerchantItemBuyRequestEvent merchantItemBuyRequestEvent) {
            // We do not worry about actually being able to afford the item for now.
            if (CanPurchase(merchantItemBuyRequestEvent.item.GetPrice())) {
                AddGold(-merchantItemBuyRequestEvent.item.GetPrice());

                items.Add(merchantItemBuyRequestEvent.item);
                Logger.Log("Added item: " + merchantItemBuyRequestEvent.item + " to the inventory.",
                    Logger.Color.RED, Player.GetInstance());
                merchantItemBuyRequestEvent.item.OnEquip();

                merchantItemBuyRequestEvent.panelButton.interactable = false; // TODO: THIS IS BAD REFACTOR A.S.A.P
            } else {
                Util.Logger.Log("CANNOT BUY THIS ITEM YOU DO NOT HAVE ENOUGHT MONEY", Util.Logger.Color.RED,
                    Player.GetInstance());
            }
        }

        private bool CanPurchase(int price) {
            return gold >= price;
        }

        private void AddGoldListener(AddGoldEvent e) {
            AddGold(e.AmountToAdd);
        }

        /// <summary>
        /// Add gold to the inventory, add negative amount to remove gold.
        /// </summary>
        /// <param name="amount"></param>
        public void AddGold(int amount) {
            gold += amount;

          
            EventBus<UIGoldChangedEvent>.Raise(new UIGoldChangedEvent(gold));
        }

        /// <summary>
        /// Add kills to the killCounter, add negative amount to remove kills.
        /// </summary>
        /// <param name="amount"></param>
        private void AddKills(int amount) {
            killCount += amount;

            EventBus<UIKillCounterChangedEvent>.Raise(new UIKillCounterChangedEvent(killCount));
        }
    }
}