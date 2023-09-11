using Events;
using UnityEngine;
using Logger = Util.Logger;

namespace Champions {
    public class ChampionLevelManager {
        private Champion m_Champion;
        private int m_CurrentLevel;
        private float m_CurrentLevelXP;
        private float m_PreviousLevelXP;
        private float m_GrowthRate = 1.2f;
        private float m_ConstantLevelXPIncrease = 5f;

        public ChampionLevelManager(Champion champion) {
            m_Champion = champion;
            m_CurrentLevel = 1;
            m_CurrentLevelXP = 5f;
            m_PreviousLevelXP = 0f;
        }

        public void CheckForLevelUp() {
            if (m_Champion.GetChampionStatistics().CurrentXP >= m_CurrentLevelXP) {
                LevelUp();
            }

            Logger.Log(
                "Current level: " + m_CurrentLevel + ", Current XP: " + m_Champion.GetChampionStatistics().CurrentXP,
                Logger.Color.PINK, m_Champion);
        }

        private void LevelUp() {
            m_PreviousLevelXP = m_CurrentLevelXP;
            m_CurrentLevel++;
            m_CurrentLevelXP *= m_GrowthRate;
            m_Champion.GetChampionStatistics().CurrentXP = 0f;

            EventBus<ChampionLevelUpEvent>.Raise(new ChampionLevelUpEvent(m_CurrentLevel, m_CurrentLevel - 1));
            Logger.Log("Leveled up to level " + m_CurrentLevel + "!", Logger.Color.PINK, m_Champion);
        }

        #region Getters and Setters

        public float CurrentLevelXP {
            get => m_CurrentLevelXP;
            set => m_CurrentLevelXP = value;
        }

        public float CurrentLevel {
            get => m_CurrentLevel;
            set => m_CurrentLevel = (int)value;
        }

        #endregion
    }
}