using System.Collections.Generic;
using System.Linq;
using Champions.Abilities;
using Champions.Abilities.Upgrades;
using EventBus;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.Serialization;
using Logger = Util.Logger;

namespace UI {
    public class LevelPanelController : MonoBehaviour {
        [SerializeField] private GameObject levelPanel;

        [SerializeField] private GameObject abilityComponentPrefab;
        [SerializeField] private GameObject upgradeComponentPrefab;

        [SerializeField] private List<AAbility> abilities = new();
        private List<ILevelPanelComponent> levelPanelComponents = new();

        private void OnEnable() {
            EventBus<ChampionLevelUpEvent>.Subscribe(OnChampionLevelUp);
        }

        private void OnDisable() {
            EventBus<ChampionLevelUpEvent>.Unsubscribe(OnChampionLevelUp);
        }

        private void Start() {
            HidePanel();
        }

        private void HidePanel() {
            levelPanel.SetActive(false);

            for (int i = levelPanelComponents.Count - 1; i >= 0; i--) {
                Destroy(levelPanelComponents[i].GetGameObject());
                levelPanelComponents.RemoveAt(i);
            }
        }

        private void ShowPanel(List<AAbility> currentChampionAbilities) {
            List<AAbility> toInstantiate = GetRandomAbilities(currentChampionAbilities);

            List<Upgrade> upgradesToInstantiate = new();

            // if (toInstantiate.Count < 1) {
            //     // Add gold
            //     EventBus<AddGoldEvent>.Raise(new AddGoldEvent(10)); // 10 gold to add
            //
            //     return;
            // }

            int diff = 3 - toInstantiate.Count;

            if (toInstantiate.Count < 3) {
                upgradesToInstantiate = GetRandomUpgrades(currentChampionAbilities, diff);
            }

            // Logger.Log("Amount of abilities to instantiate: " + toInstantiate.Count, Logger.Color.RED, this);


            // Instantiate the abilities
            for (int i = 0; i < toInstantiate.Count; i++) {
                // Instantiate the ability panel prefab
                GameObject abilityComponent = Instantiate(abilityComponentPrefab, levelPanel.transform);
                AAbility ability = toInstantiate[i];
                ILevelPanelComponent uiLevelUpComponent = abilityComponent.GetComponent<ILevelPanelComponent>();
                uiLevelUpComponent.SetLevelPanelController(this);
                // set action
                uiLevelUpComponent.SetAction(() => {
                    EventBus<ChampionAbilityChosenEvent>.Raise(
                        new ChampionAbilityChosenEvent(ability));
                    HidePanel();
                });
                uiLevelUpComponent.GetBannerImage().sprite = toInstantiate[i].GetAbilityLevelUpBannerSprite();

                levelPanelComponents.Add(uiLevelUpComponent);
            }

            Logger.Log("Amount of upgrades found: " + upgradesToInstantiate.Count, Logger.Color.BLUE, this);
            upgradesToInstantiate.ForEach(upgrade => { Logger.Log(upgrade.ToString(), Logger.Color.BLUE, this); });

            // instantiate the upgrade panel items if the difference between 3 and the amount of abilities is greater than 0
            for (int i = 0; i < upgradesToInstantiate.Count; i++) {
                GameObject upgradeComponent = Instantiate(abilityComponentPrefab, levelPanel.transform);
                ILevelPanelComponent uiUpgradeComponent = upgradeComponent.GetComponent<ILevelPanelComponent>();
                Upgrade upgrade = upgradesToInstantiate[i];
                uiUpgradeComponent.SetLevelPanelController(this);
                uiUpgradeComponent.SetAction(() => {
                    EventBus<ChampionUpgradeChosenEvent>.Raise(
                        new ChampionUpgradeChosenEvent(upgrade));
                    HidePanel();
                });

                uiUpgradeComponent.GetBannerImage().sprite = upgrade.GetUpgradeLevelUpSprite();
                (uiUpgradeComponent as UIUpgradeComponent)?.GetUpgradeText().SetText("Upgrade");

                levelPanelComponents.Add(uiUpgradeComponent);
            }

            // raise an event to stop the enemies from spawning and moving
            EventBus<UILevelUpPanelOpenEvent>.Raise(new UILevelUpPanelOpenEvent());

            // activate the panel gameobject
            levelPanel.SetActive(true);
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

        private List<Upgrade> GetRandomUpgrades(List<AAbility> currentChampionAbilities, int amountToReturn) {
            List<Upgrade> randomUpgrades = new();
            currentChampionAbilities.ForEach(ability => {
                if (ability.GetUpgrades().Count > 0) {
                    randomUpgrades.Add(ability.GetNextUpgrade());
                }
            });


            // take the amountToReturn and return that amount of random upgrades
            return randomUpgrades.OrderBy(x => Random.Range(0, 100)).Take(amountToReturn).ToList();
        }

        private void OnChampionLevelUp(ChampionLevelUpEvent e) {
            ShowPanel(e.m_ChampionAbilities);
        }
    }
}