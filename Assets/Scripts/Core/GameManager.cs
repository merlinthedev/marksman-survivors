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

            EventBus<MerchantInteractEvent>.Subscribe(OnMerchantInteract);
            EventBus<MerchantExitEvent>.Subscribe(OnMerchantExit);
        }

        private void OnDisable() {
            EventBus<UILevelUpPanelOpenEvent>.Unsubscribe(OnLevelUpPanelOpen);
            EventBus<UILevelUpPanelClosedEvent>.Unsubscribe(OnLevelUpPanelClosed);

            EventBus<UISettingsMenuClosedEvent>.Unsubscribe(OnSettingsMenuClosed);
            EventBus<UISettingsMenuOpenedEvent>.Unsubscribe(OnSettingsMenuOpen);

            EventBus<MerchantInteractEvent>.Unsubscribe(OnMerchantInteract);
            EventBus<MerchantExitEvent>.Unsubscribe(OnMerchantExit);
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

        #region Events

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

        private void OnMerchantInteract(MerchantInteractEvent obj) {
            PauseGame();
        }

        private void OnMerchantExit(MerchantExitEvent obj) {
            ResumeGame();
        }

        #endregion

        public static GameManager GetInstance() {
            return instance;
        }
    }
}