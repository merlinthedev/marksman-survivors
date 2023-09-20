using Champions.Abilities;
using EventBus;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI {
    public class UILevelUpComponent : MonoBehaviour {
        [FormerlySerializedAs("upgradeText")] [SerializeField]
        private TMP_Text abilityText;

        [SerializeField] private Image bannerImage;
        [SerializeField] private Button button;

        private LevelPanelController levelPanelController;
        private AAbility ability;
        private int index;

        private void Start() {
            button.onClick.AddListener(() => {
                EventBus<ChampionAbilityChosenEvent>.Raise(
                    new ChampionAbilityChosenEvent(ability));
            });
        }

        public void HookButton(LevelPanelController levelPanelController) {
            this.levelPanelController = levelPanelController;
        }

        public TMP_Text GetTextComponent() {
            return abilityText;
        }

        public Image GetBannerImage() {
            return bannerImage;
        }

        public int GetIndex() {
            return index;
        }

        public void SetIndex(int index) {
            this.index = index;
        }

        public void SetAbility(AAbility ability) {
            this.ability = ability;
        }
    }
}