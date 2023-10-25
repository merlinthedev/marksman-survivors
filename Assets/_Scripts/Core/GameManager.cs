using _Scripts.Core.Singleton;
using _Scripts.EventBus;

namespace _Scripts.Core {
    public class GameManager : Singleton<GameManager> {
        public bool Paused;

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


    }
}