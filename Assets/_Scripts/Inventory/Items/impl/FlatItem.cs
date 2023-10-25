using Champions;
using UnityEngine;

namespace Inventory.Items.impl {
    public class FlatItem : Item {
        [SerializeField] private int value;
        [SerializeField] private Statistic statistic;

        public override void OnEquip() {
            // ChampionStatistics cs = Player.GetInstance().GetCurrentlySelectedChampion().GetChampionStatistics();
            // Logger.Log("Calling OnEquip for item: " + this + ".", Logger.Color.RED, this);
            // Logger.Log("before: " + cs.GetStatisticByEnum(statistic).ToString() + ".", Logger.Color.RED, this);
            Player.GetInstance().GetCurrentlySelectedChampion().GetChampionStatistics()
                .AddToStatistic(statistic, value);

            // Logger.Log("after: " + cs.GetStatisticByEnum(statistic).ToString() + ".", Logger.Color.RED, this);
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