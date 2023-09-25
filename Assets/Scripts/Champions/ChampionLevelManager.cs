using EventBus;
using Logger = Util.Logger;

namespace Champions {
    public class ChampionLevelManager {
        private Champion champion;
        private int currentLevel;
        private float currentLevelXp;
        private float previousLevelXp;
        private float growthRate = 1.2f;
        private float constantLevelXpIncrease = 10f;

        public ChampionLevelManager(Champion champion) {
            this.champion = champion;
            currentLevel = 1;
            currentLevelXp = 2f;
            previousLevelXp = 0f;
        }

        public void CheckForLevelUp() {
            if (champion.GetChampionStatistics().CurrentXP >= currentLevelXp) {
                LevelUp();
            }

            Logger.Log(
                "Current level: " + currentLevel + ", Current XP: " + champion.GetChampionStatistics().CurrentXP,
                Logger.Color.PINK, champion);
        }

        private void LevelUp() {
            previousLevelXp = currentLevelXp;
            currentLevel++;
            currentLevelXp = previousLevelXp * growthRate + constantLevelXpIncrease;
            champion.GetChampionStatistics().CurrentXP = 0f;

            EventBus<ChampionLevelUpEvent>.Raise(new ChampionLevelUpEvent(currentLevel, currentLevel - 1,
                champion.GetAbilities()));
            Logger.Log("Leveled up to level " + currentLevel + "!", Logger.Color.PINK, champion);
        }

        #region Getters and Setters

        public float CurrentLevelXP {
            get => currentLevelXp;
            set => currentLevelXp = value;
        }

        public float CurrentLevel {
            get => currentLevel;
            set => currentLevel = (int)value;
        }

        #endregion
    }
}