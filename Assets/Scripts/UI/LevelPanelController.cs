using System.Collections.Generic;
using Champions.Abilities;
using EventBus;
using UnityEngine;
using UnityEngine.Serialization;
using Logger = Util.Logger;

namespace UI {
    public class LevelPanelController : MonoBehaviour {
        [SerializeField] private GameObject levelPanel;

        [SerializeField] private List<AAbility> abilities = new();

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
            levelPanel.SetActive(false);
        }

        private void ShowPanel() {
            levelPanel.SetActive(true);
        }

        private void PopulatePanel() {
            // 
        }

        private void OnChampionLevelUp(ChampionLevelUpEvent e) {
            ShowPanel();
            abilities = e.m_ChampionAbilities;
        }
    }
}