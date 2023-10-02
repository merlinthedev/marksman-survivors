using System;
using EventBus;
using UnityEngine;

namespace Core {
    public class GameManager : MonoBehaviour {
        private void OnEnable() {
            throw new NotImplementedException();
        }

        private void OnDisable() {
            throw new NotImplementedException();
        }

        private void ResumeGame() {
            EventBus<GameResumedEvent>.Raise(new GameResumedEvent());
        }

        private void PauseGame() {
            EventBus<GamePausedEvent>.Raise(new GamePausedEvent());
        }
    }
}