using System.Collections.Generic;
using Champions.Abilities;
using EventBus;
using UnityEngine;
using UnityEngine.Serialization;
using Logger = Util.Logger;

namespace UI {
    public class LevelPanelController : MonoBehaviour {
        [SerializeField] private GameObject levelPanel;

        [FormerlySerializedAs("upgradeComponentPrefab")] [SerializeField]
        private GameObject abilityComponentPrefab;

        [SerializeField] private List<AAbility> abilities = new();
        private List<UILevelUpComponent> uiUpgradeComponents = new();

        private void OnEnable() {
            EventBus<ChampionLevelUpEvent>.Subscribe(OnChampionLevelUp);
            EventBus<ChampionAbilityChosenEvent>.Subscribe(OnChampionAbilityChosen);
        }

        private void OnDisable() {
            EventBus<ChampionLevelUpEvent>.Unsubscribe(OnChampionLevelUp);
            EventBus<ChampionAbilityChosenEvent>.Unsubscribe(OnChampionAbilityChosen);
        }

        private void Start() {
            HidePanel();
        }

        private void HidePanel() {
            levelPanel.SetActive(false);

            for (int i = uiUpgradeComponents.Count - 1; i >= 0; i--) {
                Destroy(uiUpgradeComponents[i].gameObject);
                uiUpgradeComponents.RemoveAt(i);
            }
        }

        private void ShowPanel(List<AAbility> currentChampionAbilities) {
            List<AAbility> toInstantiate = GetRandomAbilities(currentChampionAbilities);

            if (toInstantiate.Count < 1) {
                // Add gold
                EventBus<AddGoldEvent>.Raise(new AddGoldEvent(10)); // 10 gold to add

                return;
            }

            levelPanel.SetActive(true);

            // Instantiate the abilities
            for (int i = 0; i < toInstantiate.Count; i++) {
                // Instantiate the ability panel prefab
                GameObject upgradeComponent = Instantiate(abilityComponentPrefab, levelPanel.transform);
                UILevelUpComponent uiLevelUpComponent = upgradeComponent.GetComponent<UILevelUpComponent>();
                uiLevelUpComponent.HookButton(this);
                uiLevelUpComponent.SetAbility(toInstantiate[i]);
                uiLevelUpComponent.GetTextComponent().SetText(toInstantiate[i].GetType().ToString());
                uiLevelUpComponent.GetBannerImage().sprite = toInstantiate[i].GetAbilityLevelUpBannerSprite();

                uiUpgradeComponents.Add(uiLevelUpComponent);
            }
        }

        private List<AAbility> GetRandomAbilities(List<AAbility> currentChampionAbilities) {
            // fetch 3 random abilities from the list
            List<AAbility> randomAbilities = new();

            int maxAttempts = 10; // Set a maximum number of attempts

            for (int i = 0; i < 3; i++) {
                int attempts = 0; // Track the number of attempts made to find a valid ability

                while (attempts < maxAttempts) {
                    int randomIndex = Random.Range(0, abilities.Count);
                    var x = abilities[randomIndex];

                    // Check if the currentChampionAbilities already contain an ability with the same keycode
                    if (currentChampionAbilities.Exists(ability => ability.GetKeyCode() == x.GetKeyCode())) {
                        attempts++;
                        continue;
                    }

                    if (randomAbilities.Contains(x)) {
                        attempts++;
                        continue;
                    }

                    randomAbilities.Add(x);
                    break; // Valid ability found, exit the while loop
                }
            }

            Logger.Log("Random abilities: " + randomAbilities.Count, Logger.Color.RED, this);

            return randomAbilities;
        }

        private void OnChampionLevelUp(ChampionLevelUpEvent e) {
            ShowPanel(e.m_ChampionAbilities);
        }

        private void OnChampionAbilityChosen(ChampionAbilityChosenEvent e) {
            HidePanel();
        }
    }
}