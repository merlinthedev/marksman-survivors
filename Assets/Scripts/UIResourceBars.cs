using Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIResourceBars : MonoBehaviour {
    [SerializeField] private Image m_XPBar;
    [SerializeField] private TMP_Text m_LevelText;

    private void OnEnable() {
        EventBus<UpdateXPBarEvent>.Subscribe(UpdateXPBar);
        EventBus<ChampionLevelUpEvent>.Subscribe(UpdateLevel);
    }

    private void OnDisable() {
        EventBus<UpdateXPBarEvent>.Unsubscribe(UpdateXPBar);
        EventBus<ChampionLevelUpEvent>.Unsubscribe(UpdateLevel);
    }

    private void UpdateXPBar(UpdateXPBarEvent e) {
        float xpPercentage = e.m_CurrentXP / e.m_TotalXP;
        m_XPBar.fillAmount = xpPercentage;
    }

    private void UpdateLevel(ChampionLevelUpEvent e) {
        m_LevelText.SetText(e.m_CurrentLevel.ToString());
    }
}