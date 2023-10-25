using _Scripts.EventBus;
using TMPro;
using UnityEngine;

namespace _Scripts.UI {
    public class GoldCounter : MonoBehaviour {
        [SerializeField] private TMP_Text goldCounterText;

        private void OnEnable() {
            EventBus<UIGoldChangedEvent>.Subscribe(OnGoldChanged);
        }

        private void OnDisable() {
            EventBus<UIGoldChangedEvent>.Unsubscribe(OnGoldChanged);
        }

        private void OnGoldChanged(UIGoldChangedEvent e) {
            goldCounterText.SetText(e.Gold.ToString());
        }
    }
}