using EventBus;
using UnityEngine;

namespace Core {
    public class GameManager : MonoBehaviour {
        private static GameManager instance;
        public bool Paused { get; private set; }

        private void OnEnable() {
            EventBus<UILevelUpPanelOpenEvent>.Subscribe(OnLevelUpPanelOpen);
            EventBus<UILevelUpPanelClosedEvent>.Subscribe(OnLevelUpPanelClosed);
        }

        private void OnDisable() {
            EventBus<UILevelUpPanelOpenEvent>.Unsubscribe(OnLevelUpPanelOpen);
            EventBus<UILevelUpPanelClosedEvent>.Unsubscribe(OnLevelUpPanelClosed);
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

        public static GameManager GetInstance() {
            return instance;
        }
    }
}