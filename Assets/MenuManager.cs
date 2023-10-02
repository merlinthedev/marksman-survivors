using UnityEngine;
using EventBus;

public class MenuManager : MonoBehaviour {
    [SerializeField] public GameObject settings;

    private void OnEnable() {
        EventBus<ToggleSettingsMenuEvent>.Subscribe(ToggleSettingsMenu);
    }

    private void OnDisable() {
        EventBus<ToggleSettingsMenuEvent>.Unsubscribe(ToggleSettingsMenu);
    }

    private void ToggleSettingsMenu(ToggleSettingsMenuEvent e) {
        settings.SetActive(!settings.activeSelf);

        if (settings.activeSelf) {
            EventBus<UISettingsMenuOpenedEvent>.Raise(new UISettingsMenuOpenedEvent());
        }
        else {
            EventBus<UISettingsMenuClosedEvent>.Raise(new UISettingsMenuClosedEvent());
        }
    }
}