using EventBus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class UIResourceBars : MonoBehaviour {
        [Header("References")]
        [SerializeField] private Image healthBar;

        [SerializeField] private Image manaBar;

        [SerializeField] private Image xpBar;
        [SerializeField] private TMP_Text levelText;

        private void OnEnable() {
            EventBus<UpdateResourceBarEvent>.Subscribe(UpdateResourceBar);
            EventBus<ChampionLevelUpEvent>.Subscribe(UpdateLevel);
        }

        private void OnDisable() {
            EventBus<UpdateResourceBarEvent>.Unsubscribe(UpdateResourceBar);
            EventBus<ChampionLevelUpEvent>.Unsubscribe(UpdateLevel);
        }

        private void UpdateResourceBar(UpdateResourceBarEvent e) {
            float percentage = e.current / e.total;

            switch (e.type) {
                case "Health":
                    healthBar.fillAmount = percentage;
                    break;
                case "Mana":
                    manaBar.fillAmount = percentage;
                    break;
                case "XP":
                    xpBar.fillAmount = percentage;
                    break;
            }
        }

        private void UpdateLevel(ChampionLevelUpEvent e) {
            levelText.SetText(e.m_CurrentLevel.ToString());
        }
    }
}