using System.Collections.Generic;
using EventBus;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class UICooldowns : MonoBehaviour {
        [Header("References")]
        [SerializeField] private Image m_LMB;
        [SerializeField] private Image m_RMB;
        [SerializeField] private Image m_Q;
        [SerializeField] private Image m_W;
        [SerializeField] private Image m_E;
        [SerializeField] private Image m_R;

        private List<AbilityCooldown> m_AbilityCooldowns = new List<AbilityCooldown>();

        private class AbilityCooldown {
            public float m_TotalDuration;
            public float m_CurrentTime;
            public Image m_Image;
        }

        private void OnEnable() {
            EventBus<ChampionAbilityUsedEvent>.Subscribe(HandleAbilityCooldowns);
        }

        private void OnDisable() {
            EventBus<ChampionAbilityUsedEvent>.Unsubscribe(HandleAbilityCooldowns);
        }

        private void HandleAbilityCooldowns(ChampionAbilityUsedEvent e) {
            KeyCode m_KeyCode;
            float duration;

            if (e.m_Ability == null) {
                m_KeyCode = e.m_KeyCode;
                duration = e.m_Duration;
            } else {
                m_KeyCode = e.m_Ability.GetKeyCode();
                duration = e.m_Ability.GetAbilityCooldown();
            }

            switch (m_KeyCode) {
                case KeyCode.Mouse0:
                    m_AbilityCooldowns.Add(new AbilityCooldown() {
                        m_TotalDuration = duration,
                        m_CurrentTime = duration,
                        m_Image = m_LMB
                    });
                    break;
                case KeyCode.Mouse1:
                    m_AbilityCooldowns.Add(new AbilityCooldown() {
                        m_TotalDuration = duration,
                        m_CurrentTime = duration,
                        m_Image = m_RMB
                    });
                    break;
                case KeyCode.Q:
                    m_AbilityCooldowns.Add(new AbilityCooldown() {
                        m_TotalDuration = duration,
                        m_CurrentTime = duration,
                        m_Image = m_Q
                    });
                    break;
                case KeyCode.W:
                    m_AbilityCooldowns.Add(new AbilityCooldown() {
                        m_TotalDuration = duration,
                        m_CurrentTime = duration,
                        m_Image = m_W
                    });
                    break;
                case KeyCode.E:
                    m_AbilityCooldowns.Add(new AbilityCooldown() {
                        m_TotalDuration = duration,
                        m_CurrentTime = duration,
                        m_Image = m_E
                    });
                    break;
                case KeyCode.R:
                    m_AbilityCooldowns.Add(new AbilityCooldown() {
                        m_TotalDuration = duration,
                        m_CurrentTime = duration,
                        m_Image = m_R
                    });
                    break;
            }
        }

        private void Update() {
            for (int i = 0; i < m_AbilityCooldowns.Count; i++) {
                AbilityCooldown abilityCooldown = m_AbilityCooldowns[i];
                abilityCooldown.m_CurrentTime -= Time.deltaTime;
                float cooldownPercentage = abilityCooldown.m_CurrentTime / abilityCooldown.m_TotalDuration;
                abilityCooldown.m_Image.fillAmount = cooldownPercentage;
                if (abilityCooldown.m_CurrentTime <= 0) {
                    m_AbilityCooldowns.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
