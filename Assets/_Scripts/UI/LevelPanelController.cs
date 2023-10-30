using _Scripts.Champions.Abilities;
using _Scripts.Champions.Abilities.Upgrades;
using _Scripts.EventBus;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Logger = _Scripts.Util.Logger;

namespace _Scripts.UI {
    public class LevelPanelController : MonoBehaviour {
        [SerializeField] private GameObject levelPanel;

        [SerializeField] private GameObject abilityComponentPrefab;
        [SerializeField] private GameObject upgradeComponentPrefab;

        private List<Ability> abilitiesToLevelUp;
        private bool leveledUp;
        [SerializeField] private TMP_Text prompt;

        [SerializeField] private List<Ability> abilities = new();
        private List<ILevelPanelComponent> levelPanelComponents = new();

        private int stackedLevelUp = 0;

        private void OnEnable() {
            EventBus<ChampionLevelUpEvent>.Subscribe(OnChampionLevelUp);
            EventBus<ShowLevelUpPanelEvent>.Subscribe(ShowPanel);
            EventBus<LevelUpPromptEvent>.Subscribe(ShowPrompt);
        }

        private void OnDisable() {
            EventBus<ChampionLevelUpEvent>.Unsubscribe(OnChampionLevelUp);
            EventBus<ShowLevelUpPanelEvent>.Unsubscribe(ShowPanel);
            EventBus<LevelUpPromptEvent>.Unsubscribe(ShowPrompt);
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

        private void ShowPrompt(LevelUpPromptEvent e) {
            prompt.gameObject.SetActive(e.open);
        }

        private void ShowPanel(ShowLevelUpPanelEvent e) {
            if (!leveledUp) {
                Logger.Log("Did not level up, returning", Logger.Color.RED, this);
                return;
            }

            List<Ability> toInstantiate = GetRandomAbilities(abilitiesToLevelUp);

            List<Upgrade> upgradesToInstantiate = new();

            // if (toInstantiate.Count < 1) {
            //     // Add gold
            //     EventBus<AddGoldEvent>.Raise(new AddGoldEvent(10)); // 10 gold to add
            //
            //     return;
            // }

            int diff = 3 - toInstantiate.Count;

            if (toInstantiate.Count < 3) {
                upgradesToInstantiate = GetRandomUpgrades(abilitiesToLevelUp, diff);
            }

            // Logger.Log("Amount of abilities to instantiate: " + toInstantiate.Count, Logger.Color.RED, this);


            // Instantiate the abilities
            for (int i = 0; i < toInstantiate.Count; i++) {
                // Instantiate the ability panel prefab
                GameObject abilityComponent = Instantiate(abilityComponentPrefab, levelPanel.transform);
                Ability ability = toInstantiate[i];
                ILevelPanelComponent uiLevelUpComponent = abilityComponent.GetComponent<ILevelPanelComponent>();
                uiLevelUpComponent.SetLevelPanelController(this);
                // set action
                uiLevelUpComponent.SetAction(() => {
                    EventBus<ChampionAbilityChosenEvent>.Raise(
                        new ChampionAbilityChosenEvent(ability, true));
                    EventBus<ChampionAbilityUsedEvent>.Raise(new ChampionAbilityUsedEvent(ability));
                    HidePanel();

                    if (stackedLevelUp >= 1) {
                        EventBus<LevelUpPromptEvent>.Raise(new LevelUpPromptEvent(true));
                    }
                });
                uiLevelUpComponent.GetBannerImage().sprite = toInstantiate[i].GetAbilityLevelUpBannerSprite();

                levelPanelComponents.Add(uiLevelUpComponent);
            }

            // Logger.Log("Amount of upgrades found: " + upgradesToInstantiate.Count, Logger.Color.BLUE, this);
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

                    if (stackedLevelUp >= 1) {
                        EventBus<LevelUpPromptEvent>.Raise(new LevelUpPromptEvent(true));
                    }
                });

                uiUpgradeComponent.GetBannerImage().sprite = upgrade.GetUpgradeLevelUpSprite();
                (uiUpgradeComponent as UIUpgradeComponent)?.GetUpgradeText().SetText("Upgrade");

                levelPanelComponents.Add(uiUpgradeComponent);
            }

            stackedLevelUp--;

            // raise an event to stop the enemies from spawning and moving
            EventBus<UILevelUpPanelOpenEvent>.Raise(new UILevelUpPanelOpenEvent());

            // activate the panel gameobject
            levelPanel.SetActive(true);

            // set the leveled up bool to false and remove the level up prompt
            leveledUp = stackedLevelUp >= 1;
            EventBus<LevelUpPromptEvent>.Raise(new LevelUpPromptEvent(false));
        }

        private List<Ability> GetRandomAbilities(List<Ability> currentChampionAbilities) {
            // fetch 3 random abilities from the list
            List<Ability> randomAbilities = new();

            int maxAttempts = 20; // Set a maximum number of attempts

            // can only unlock ultimate when all the other abilities are unlocked
            bool canUnlockUltimate = currentChampionAbilities.Count >= 4;

            for (int i = 0; i < 3; i++) {
                int attempts = 0; // Track the number of attempts made to find a valid ability

                while (attempts < maxAttempts) {
                    int randomIndex = Random.Range(0, abilities.Count);
                    var x = abilities[randomIndex];

                    if (!canUnlockUltimate && x.abilityType == Ability.AbilityType.ULTIMATE) {
                        attempts++;
                        continue;
                    }

                    // Check if the currentChampionAbilities already contain an ability with the same keycode
                    if (currentChampionAbilities.Exists(ability => ability.abilityType == Ability.AbilityType.BASIC)) {
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

        private List<Upgrade> GetRandomUpgrades(List<Ability> currentChampionAbilities, int amountToReturn) {
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
            EventBus<LevelUpPromptEvent>.Raise(new LevelUpPromptEvent(true));
            abilitiesToLevelUp = e.m_ChampionAbilities;
            leveledUp = true;
            stackedLevelUp++;
            Logger.Log("stackedLevelUp: " + stackedLevelUp, Logger.Color.RED, this);
        }
    }
}