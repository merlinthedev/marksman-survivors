using System.Collections.Generic;
using System.Linq;
using Champions.Abilities;
using EventBus;
using UnityEngine;
using Logger = Util.Logger;

namespace UI {
    public class LevelPanelController : MonoBehaviour {
        [SerializeField] private GameObject levelPanel;

        [SerializeField] private List<AAbility> abilities = new();
        private List<AAbility> randomAbilities = new();

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
                    EventBus<ChampionAbilityChosenEvent>.Raise(new ChampionAbilityChosenEvent(randomAbilities[0]));
                    break;
                case 1:
                    Logger.Log("Clicked the second ability", Logger.Color.PINK, this);
                    EventBus<ChampionAbilityChosenEvent>.Raise(new ChampionAbilityChosenEvent(randomAbilities[1]));
                    break;
                case 2:
                    Logger.Log("Clicked the third ability", Logger.Color.PINK, this);
                    EventBus<ChampionAbilityChosenEvent>.Raise(new ChampionAbilityChosenEvent(randomAbilities[2]));
                    break;
            }

            HidePanel();
        }

        private void HidePanel() {
            levelPanel.SetActive(false);
        }

        private void ShowPanel(List<AAbility> currentChampionAbilities) {
            levelPanel.SetActive(true);
            PopulatePanel(currentChampionAbilities);
        }

        private void PopulatePanel(List<AAbility> currentChampionAbilities) {
            // fetch 3 random abilities from the list
            randomAbilities.Clear();
            for (int i = 0; i < 3; i++) {
                int randomIndex = Random.Range(0, abilities.Count);
                var x = abilities[randomIndex];
                // check if the currentChampionAbilites already contains a ability with the same keycode
                if (currentChampionAbilities.Exists(ability => ability.GetKeyCode() == x.GetKeyCode())) {
                    i--;
                    continue;
                }

                if (randomAbilities.Contains(x)) {
                    i--;
                    continue;
                }

                randomAbilities.Add(x);
            }

            List<UIUpgradeComponent> uiUpgradeComponents = GetComponentsInChildren<UIUpgradeComponent>().ToList();

            for (int i = 0; i < randomAbilities.Count; i++) {
                uiUpgradeComponents[i].GetTextComponent().SetText(randomAbilities[i].GetType().ToString());
                uiUpgradeComponents[i].GetBannerImage().sprite = randomAbilities[i].GetAbilityLevelUpBannerSprite();
            }
        }

        private void OnChampionLevelUp(ChampionLevelUpEvent e) {
            ShowPanel(e.m_ChampionAbilities);
        }
    }
}