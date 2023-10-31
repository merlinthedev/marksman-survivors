using _Scripts.Champions.Abilities;
using _Scripts.EventBus;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Champions.Kitegirl.Abilities;
using TMPro;
using UnityEngine;

namespace _Scripts.UI {
    public class LevelPanelController : MonoBehaviour {
        [SerializeField] private GameObject levelPanel;
        [SerializeField] private TMP_Text prompt;


        private List<Ability> allAbilities = new();


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

        private void HidePanel() {
            levelPanel.SetActive(false);
        }

        private void ShowPrompt(LevelUpPromptEvent e) {
            prompt.gameObject.SetActive(e.open);
        }

        private void ShowPanel(ShowLevelUpPanelEvent e) {
            // activate the panel gameobject
            levelPanel.SetActive(true);

            EventBus<LevelUpPromptEvent>.Raise(new LevelUpPromptEvent(false));
        }


        private void OnChampionLevelUp(ChampionLevelUpEvent e) {
            EventBus<LevelUpPromptEvent>.Raise(new LevelUpPromptEvent(true));
        }
    }
}