using System;
using EventBus;
using UnityEngine;

namespace Core {
    public class GameManager : MonoBehaviour {
        private void OnEnable() {
            EventBus<UILevelUpPanelOpenEvent>.Subscribe(OnLevelUpPanelOpen);
            EventBus<UILevelUpPanelClosedEvent>.Subscribe(OnLevelUpPanelClosed);
        }


        private void OnDisable() {
            EventBus<UILevelUpPanelOpenEvent>.Unsubscribe(OnLevelUpPanelOpen);
            EventBus<UILevelUpPanelClosedEvent>.Unsubscribe(OnLevelUpPanelClosed);
        }


        private void ResumeGame() {
            EventBus<GameResumedEvent>.Raise(new GameResumedEvent());
        }

        private void PauseGame() {
            EventBus<GamePausedEvent>.Raise(new GamePausedEvent());
        }

        private void OnLevelUpPanelOpen(UILevelUpPanelOpenEvent obj) {
            PauseGame();
        }

        private void OnLevelUpPanelClosed(UILevelUpPanelClosedEvent obj) {
            ResumeGame();
        }
    }
}