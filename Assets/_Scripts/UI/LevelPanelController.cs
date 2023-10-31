using _Scripts.Champions.Abilities;
using _Scripts.EventBus;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Champions.Kitegirl.Abilities;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace _Scripts.UI {
    public class LevelPanelController : MonoBehaviour {
        [SerializeField] private GameObject levelPanel;
        [SerializeField] private GameObject abilityPrefab;
        [SerializeField] private TMP_Text prompt;


        private List<Ability> allAbilities = new();
        private List<Ability> currentChampionAbilities = new();


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

            LoadPrefabs();
        }

        private void LoadPrefabs() {
            allAbilities = Resources.LoadAll<Ability>("Prefab/Kitegirl/Abilities").ToList();

            for (int i = allAbilities.Count - 1; i >= 0; i--) {
                if (allAbilities[i] is AutoAttack) {
                    allAbilities.RemoveAt(i);
                }
            }

            allAbilities.ForEach(Debug.Log);
        }

        private List<Ability> GetRandomAbilities() {
            List<Ability> randomAbilities = new();

            bool canUnlockUltimate = currentChampionAbilities.Count >= 5;


            for (int i = 0; i < 3; i++) {
                int randomIndex = Random.Range(0, allAbilities.Count);

                if (currentChampionAbilities.Contains(allAbilities[randomIndex])) {
                    i--;
                    continue;
                }

                if (randomAbilities.Contains(allAbilities[randomIndex])) {
                    i--;
                    continue;
                }

                if (!canUnlockUltimate && allAbilities[randomIndex].abilityType == Ability.AbilityType.ULTIMATE) {
                    i--;
                    continue;
                }


                randomAbilities.Add(allAbilities[randomIndex]);
            }

            return randomAbilities;
        }

        private void HidePanel() {
            EventBus<UILevelUpPanelClosedEvent>.Raise(new());
            levelPanel.SetActive(false);
        }

        private void ShowPrompt(LevelUpPromptEvent e) {
            prompt.gameObject.SetActive(e.open);
        }

        private void RemovePreviousAbilities() {
            for (int i = 0; i < levelPanel.transform.childCount; i++) {
                Destroy(levelPanel.transform.GetChild(i).gameObject);
            }
        }

        private void ShowPanel(ShowLevelUpPanelEvent e) {
            RemovePreviousAbilities();
            EventBus<UILevelUpPanelOpenEvent>.Raise(new());
            var randomAbilities = GetRandomAbilities();

            Debug.Log("random abilities count: " + randomAbilities.Count, this);

            for (int i = 0; i < randomAbilities.Count; i++) {
                var ability = randomAbilities[i];
                var inst = Instantiate(ability.GetAbilityDescriptionPrefab().transform.GetChild(0),
                    levelPanel.transform);
                AbilityLevelUpController controller = inst.GetComponent<AbilityLevelUpController>();
                if (controller == null) {
                    Debug.LogError(inst.name + " is missing AbilityLevelUpController");
                    Debug.LogError(inst.name + " is missing AbilityLevelUpController");
                    return;
                }

                controller.SetAction(() => {
                    EventBus<ChampionAbilityChosenEvent>.Raise(new ChampionAbilityChosenEvent(ability, true));
                    HidePanel();
                });
            }

            // activate the panel gameobject
            levelPanel.SetActive(true);

            EventBus<LevelUpPromptEvent>.Raise(new LevelUpPromptEvent(false));
        }


        private void OnChampionLevelUp(ChampionLevelUpEvent e) {
            EventBus<LevelUpPromptEvent>.Raise(new LevelUpPromptEvent(true));
            currentChampionAbilities = e.ChampionAbilities;
        }
    }
}