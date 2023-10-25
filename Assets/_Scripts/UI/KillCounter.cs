using _Scripts.EventBus;
using TMPro;
using UnityEngine;

namespace _Scripts.UI {
    public class KillCounter : MonoBehaviour {
        [SerializeField] private TMP_Text killCounterText;

        private void OnEnable() {
            EventBus<UIKillCounterChangedEvent>.Subscribe(OnKillCounterChanged);
        }

        private void OnDisable() {
            EventBus<UIKillCounterChangedEvent>.Unsubscribe(OnKillCounterChanged);
        }

        private void OnKillCounterChanged(UIKillCounterChangedEvent e) {
            killCounterText.SetText(e.KillCount.ToString());
        }
    }
}