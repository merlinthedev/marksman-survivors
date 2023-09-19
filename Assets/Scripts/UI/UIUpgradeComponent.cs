using EventBus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class UIUpgradeComponent : MonoBehaviour {
        [SerializeField] private TMP_Text upgradeText;
        [SerializeField] private Image bannerImage;
        [SerializeField] private Button button;

        private LevelPanelController levelPanelController;
        private int index;

        private void Start() {
            button.onClick.AddListener(() => {
                EventBus<ChampionAbilityChosenEvent>.Raise(
                    new ChampionAbilityChosenEvent(levelPanelController.GetAbilityFromIndex(index)));
            });
        }

        public void HookButton(LevelPanelController levelPanelController) {
            this.levelPanelController = levelPanelController;
        }

        public TMP_Text GetTextComponent() {
            return upgradeText;
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
    }
}