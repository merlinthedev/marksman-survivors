using Events;

public class ChampionLevelManager {

    private Champion m_Champion;
    private int m_CurrentLevel;
    private float m_CurrentLevelXP;
    private float m_PreviousLevelXP;
    private float m_GrowthRate = 1.1f;

    public ChampionLevelManager(Champion champion) {
        m_Champion = champion;
        m_CurrentLevel = 1;
        m_CurrentLevelXP = 100f;
        m_PreviousLevelXP = 0f;
    }

    public void CheckForLevelUp() {
        if (m_Champion.GetChampionStatistics().CurrentXP >= m_CurrentLevelXP) {
            LevelUp();
        }
    }

    private void LevelUp() {
        m_PreviousLevelXP = m_CurrentLevelXP;
        m_CurrentLevel++;
        m_CurrentLevelXP *= m_GrowthRate;
        m_Champion.GetChampionStatistics().CurrentXP = 0f;

        EventBus<ChampionLevelUpEvent>.Raise(new ChampionLevelUpEvent(m_CurrentLevel, m_CurrentLevel - 1));
    }


}