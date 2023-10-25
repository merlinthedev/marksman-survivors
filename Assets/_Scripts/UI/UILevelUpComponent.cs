using _Scripts.EventBus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI {
    public class UILevelUpComponent : MonoBehaviour, ILevelPanelComponent {
        [SerializeField] private TMP_Text abilityText;

        [SerializeField] private Image bannerImage;
        [SerializeField] private Button button;

        private LevelPanelController levelPanelController;
        private int index;
        private System.Action action;

        private void Start() {
            button.onClick.AddListener(() => {
                action?.Invoke();
                EventBus<UILevelUpPanelClosedEvent>.Raise(new UILevelUpPanelClosedEvent());
            });
        }

        public void SetLevelPanelController(LevelPanelController levelPanelController) {
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

        public GameObject GetGameObject() {
            return gameObject;
        }

        public void SetIndex(int index) {
            this.index = index;
        }

        public void SetAction(System.Action action) {
            this.action = action;
        }
    }
}