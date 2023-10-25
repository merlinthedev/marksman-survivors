using _Scripts.Champions;
using UnityEngine;

namespace _Scripts.Inventory.Items.impl {
    public class PercentageItem : Item {
        [SerializeField] private float percentage;
        [SerializeField] private Statistic statistic;

        private void Initialize() {
            percentage = percentage / 100 + 1;
        }

        public override void OnEquip() {
            Initialize();
            // ChampionStatistics cs = Player.GetInstance().GetCurrentlySelectedChampion().GetChampionStatistics();
            // Logger.Log("Before;  " + cs.GetStatisticByEnum(statistic), Logger.Color.RED, this);

            Player.GetInstance().GetCurrentlySelectedChampion().GetChampionStatistics()
                .MultiplyStatisticByPercentage(statistic, percentage);

            // Logger.Log("AFter: " + cs.GetStatisticByEnum(statistic), Logger.Color.RED, this);
        }

        public override void OnUnequip() {
            Player.GetInstance().GetCurrentlySelectedChampion().GetChampionStatistics()
                .DivideStatisticByPercentage(statistic, percentage);
        }
    }
}