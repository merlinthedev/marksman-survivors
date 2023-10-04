﻿using Champions;
using UnityEngine;
using Logger = Util.Logger;

namespace Inventory.Items.impl {
    public class FlatItem : Item {
        [SerializeField] private int value;
        [SerializeField] private Statistic statistic;

        public override void OnEquip() {
            Logger.Log("Calling OnEquip for item: " + this + ".", Logger.Color.RED, this);
            Player.GetInstance().GetCurrentlySelectedChampion().GetChampionStatistics()
                .AddToStatistic(statistic, value);
        }

        public override void OnUnequip() {
            Player.GetInstance().GetCurrentlySelectedChampion().GetChampionStatistics()
                .AddToStatistic(statistic, -value);
        }

        #region Getters & Setters

        private int GetValue() {
            return value;
        }

        private Statistic GetStatistic() {
            return statistic;
        }

        public override string ToString() {
            return base.ToString() + ", value: " + value + ", statistic: " + statistic + ".";
        }

        #endregion
    }
}