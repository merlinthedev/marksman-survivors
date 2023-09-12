using EventBus;
using TMPro;
using UnityEngine;

namespace UI {
    public class KillCounter : MonoBehaviour {
        [SerializeField] private TMP_Text killCounterText;
        private int killCounter = 0;

        private void OnEnable() {
            EventBus<EnemyKilledEvent>.Subscribe(OnEnemyKilled);
        }

        private void OnDisable() {
            EventBus<EnemyKilledEvent>.Unsubscribe(OnEnemyKilled);
        }

        private void OnEnemyKilled(EnemyKilledEvent e) {
            killCounter++;
            killCounterText.SetText(killCounter.ToString());
        }
    }
}