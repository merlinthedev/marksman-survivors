using EventBus;
using Unity.VisualScripting;
using UnityEngine;
using Logger = Util.Logger;

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
            HidePanel();
        }

        public void OnButtonClick(int index) {
            switch (index) {
                case 0:
                    Logger.Log("Clicked the first ability", Logger.Color.PINK, this);
                    EventBus<ChampionAbilityChosenEvent>.Raise(new ChampionAbilityChosenEvent());
                    break;
                case 1:
                    Logger.Log("Clicked the second ability", Logger.Color.PINK, this);
                    EventBus<ChampionAbilityChosenEvent>.Raise(new ChampionAbilityChosenEvent());
                    break;
                case 2:
                    Logger.Log("Clicked the third ability", Logger.Color.PINK, this);
                    EventBus<ChampionAbilityChosenEvent>.Raise(new ChampionAbilityChosenEvent());
                    break;
            }

            HidePanel();
        }

        private void HidePanel() {
            m_LevelPanel.SetActive(false);
        }

        private void ShowPanel() {
            m_LevelPanel.SetActive(true);
        }

        private void OnChampionLevelUp(ChampionLevelUpEvent e) {
            ShowPanel();
        }
    }
}