using System;
using EventBus;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class UIUpgradeComponent : MonoBehaviour, ILevelPanelComponent {
        [SerializeField] private Image bannerImage;
        [SerializeField] private Button button;
        private LevelPanelController levelPanelController;
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

        public Image GetBannerImage() {
            return bannerImage;
        }

        public GameObject GetGameObject() {
            return gameObject;
        }

        public void SetAction(System.Action action) {
            this.action = action;
        }
    }
}