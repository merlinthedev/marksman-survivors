using System.Collections;
using EventBus;
using TMPro;
using UnityEngine;

namespace UI {
    public class TimeHandler : MonoBehaviour {
        [SerializeField] private TMP_Text timeText;

        private int seconds;
        private int minutes;


        private void OnEnable() {
            EventBus<GamePausedEvent>.Subscribe(OnGamePaused);
            EventBus<GameResumedEvent>.Subscribe(OnGameResumed);
        }

        private void OnDisable() {
            EventBus<GamePausedEvent>.Unsubscribe(OnGamePaused);
            EventBus<GameResumedEvent>.Unsubscribe(OnGameResumed);
        }

        private void Start() {
            StartCoroutine(Clock());
        }

        private IEnumerator Clock() {
            while (true) {
                // TODO: replace later with some type of gamerunning boolean
                seconds++;
                if (seconds == 60) {
                    minutes++;
                    seconds = 0;
                }

                timeText.text = $"{minutes:00}:{seconds:00}";

                yield return new WaitForSeconds(1f);
            }
        }


        private void OnGamePaused(GamePausedEvent e) {
            StopAllCoroutines();
        }

        private void OnGameResumed(GameResumedEvent e) {
            StartCoroutine(Clock());
        }
    }
}