using EventBus;
using UnityEngine;
using Event = UnityEngine.Event;

namespace Core {
    public class GameManager : MonoBehaviour {
        private static GameManager instance;
        public bool Paused { get; private set; }

        private void OnEnable() {
            EventBus<UILevelUpPanelOpenEvent>.Subscribe(OnLevelUpPanelOpen);
            EventBus<UILevelUpPanelClosedEvent>.Subscribe(OnLevelUpPanelClosed);

            EventBus<UISettingsMenuClosedEvent>.Subscribe(OnSettingsMenuClosed);
            EventBus<UISettingsMenuOpenedEvent>.Subscribe(OnSettingsMenuOpen);
        }

        private void OnDisable() {
            EventBus<UILevelUpPanelOpenEvent>.Unsubscribe(OnLevelUpPanelOpen);
            EventBus<UILevelUpPanelClosedEvent>.Unsubscribe(OnLevelUpPanelClosed);

            EventBus<UISettingsMenuClosedEvent>.Unsubscribe(OnSettingsMenuClosed);
            EventBus<UISettingsMenuOpenedEvent>.Unsubscribe(OnSettingsMenuOpen);
        }

        private void Start() {
            if (instance != null) {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        private void ResumeGame() {
            EventBus<GameResumedEvent>.Raise(new GameResumedEvent());
            Paused = false;
        }

        private void PauseGame() {
            EventBus<GamePausedEvent>.Raise(new GamePausedEvent());
            Paused = true;
        }

        private void OnLevelUpPanelOpen(UILevelUpPanelOpenEvent obj) {
            PauseGame();
        }

        private void OnLevelUpPanelClosed(UILevelUpPanelClosedEvent obj) {
            ResumeGame();
        }

        private void OnSettingsMenuOpen(UISettingsMenuOpenedEvent obj) {
            PauseGame();
        }

        private void OnSettingsMenuClosed(UISettingsMenuClosedEvent obj) {
            ResumeGame();
        }

        public static GameManager GetInstance() {
            return instance;
        }
    }
}