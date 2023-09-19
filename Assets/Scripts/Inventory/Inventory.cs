using EventBus;

namespace Inventory {
    public class Inventory {
        private int gold;
        private int killCount;

        public Inventory() {
            gold = 0;
            killCount = 0;

            // Subscribe to event
            EventBus<EnemyKilledEvent>.Subscribe(OnEnemyKilled);
        }

        ~Inventory() {
            // Unsubscribe from event
            EventBus<EnemyKilledEvent>.Unsubscribe(OnEnemyKilled);
        }

        private void OnEnemyKilled(EnemyKilledEvent enemyKilledEvent) {
            AddKills(1);

            AddGold(1);
        }

        /// <summary>
        /// Add gold to the inventory, add negative amount to remove gold.
        /// </summary>
        /// <param name="amount"></param>
        private void AddGold(int amount) {
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