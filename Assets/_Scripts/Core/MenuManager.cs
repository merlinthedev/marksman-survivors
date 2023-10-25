using _Scripts.EventBus;
using UnityEngine;

namespace _Scripts.Core {
    public class MenuManager : MonoBehaviour {
        [SerializeField] public GameObject settings;
        [SerializeField] public Bamischijf bamischijf;
        [SerializeField] public GameObject cheatsPanel;

        private void OnEnable() {
            EventBus<ToggleMenuEvent>.Subscribe(ToggleSettingsMenu);
        }

        private void OnDisable() {
            EventBus<ToggleMenuEvent>.Unsubscribe(ToggleSettingsMenu);
        }

        private void ToggleSettingsMenu(ToggleMenuEvent e) {
            if (e.menu == "settings") settings.SetActive(!settings.activeSelf);
            else if (e.menu == "cheats" && !cheatsPanel.activeSelf) cheatsPanel.SetActive(!cheatsPanel.activeSelf);
            else if (e.menu == "cheats" && cheatsPanel.activeSelf) bamischijf.CloseMenu();

            if (settings.activeSelf || cheatsPanel.activeSelf) {
                EventBus<UISettingsMenuOpenedEvent>.Raise(new UISettingsMenuOpenedEvent());
            } else {
                EventBus<UISettingsMenuClosedEvent>.Raise(new UISettingsMenuClosedEvent());
            }
        }
    }
}