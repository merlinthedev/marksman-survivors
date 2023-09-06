using Events;
using UnityEngine;

namespace UI {
    public class LevelPanelController : MonoBehaviour {
        [SerializeField] private GameObject m_LevelPanel;


        private void OnEnable() {
            EventBus<ChampionLevelUpEvent>.Subscribe(OnChampionLevelUp);
        }

        private void OnDisable() {
            EventBus<ChampionLevelUpEvent>.Unsubscribe(OnChampionLevelUp);
        }

        private void Start() {
            m_LevelPanel.SetActive(false);
        }

        private void OnChampionLevelUp(ChampionLevelUpEvent e) {
            m_LevelPanel.SetActive(true);
        }
    }
}