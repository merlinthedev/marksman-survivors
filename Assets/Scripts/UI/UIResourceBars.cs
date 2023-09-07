using Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIResourceBars : MonoBehaviour {
    [Header("References")]
    [SerializeField] private Image m_HealthBar;
    [SerializeField] private Image m_XPBar;
    [SerializeField] private TMP_Text m_LevelText;

    private void OnEnable() {
        EventBus<UpdateResourceBarEvent>.Subscribe(UpdateResourceBar);
        EventBus<ChampionLevelUpEvent>.Subscribe(UpdateLevel);
    }

    private void OnDisable() {
        EventBus<UpdateResourceBarEvent>.Unsubscribe(UpdateResourceBar);
        EventBus<ChampionLevelUpEvent>.Unsubscribe(UpdateLevel);
    }

    private void UpdateResourceBar(UpdateResourceBarEvent e) {
        float percentage = e.m_Current / e.m_Total;

        switch (e.m_Type) {
            case "Health":
                m_HealthBar.fillAmount = percentage;
                break;
            case "Mana":
                break;
            case "XP":
                m_XPBar.fillAmount = percentage;
                break;
        }

    }

    private void UpdateLevel(ChampionLevelUpEvent e) {
        m_LevelText.SetText(e.m_CurrentLevel.ToString());
    }
}