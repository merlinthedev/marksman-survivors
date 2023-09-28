using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventBus;

public class MenuManager : MonoBehaviour
{
    [SerializeField] public GameObject settings;

    private void OnEnable() {
        EventBus<ToggleSettingsMenuEvent>.Subscribe(ToggleSettingsMenu);
    }

    private void OnDisable() {
        EventBus<ToggleSettingsMenuEvent>.Unsubscribe(ToggleSettingsMenu);
    }

    private void ToggleSettingsMenu(ToggleSettingsMenuEvent e) {
        settings.SetActive(!settings.activeSelf);
    }
}
