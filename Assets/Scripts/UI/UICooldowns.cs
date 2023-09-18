using System.Collections.Generic;
using EventBus;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class UICooldowns : MonoBehaviour {
        [Header("References")]
        [SerializeField] private Image lmbImage;

        [SerializeField] private Image rmbImage;
        [SerializeField] private Image mQ;
        [SerializeField] private Image mW;
        [SerializeField] private Image mE;
        [SerializeField] private Image mR;

        private List<AbilityCooldown> mAbilityCooldowns = new();

        private class AbilityCooldown {
            public float MTotalDuration;
            public float MCurrentTime;
            public Image MImage;
        }

        private void OnEnable() {
            EventBus<ChampionAbilityUsedEvent>.Subscribe(HandleAbilityCooldowns);
        }

        private void OnDisable() {
            EventBus<ChampionAbilityUsedEvent>.Unsubscribe(HandleAbilityCooldowns);
        }

        private void Start() {
            // fill all the cooldowns fully
            // m_LMB.fillAmount = 1;
            rmbImage.fillAmount = 1;
            mQ.fillAmount = 1;
            mW.fillAmount = 1;
            mE.fillAmount = 1;
            mR.fillAmount = 1;
        }

        private void HandleAbilityCooldowns(ChampionAbilityUsedEvent e) {
            KeyCode mKeyCode;
            float duration;

            if (e.m_Ability == null) {
                mKeyCode = e.m_KeyCode;
                duration = e.m_Duration;
            }
            else {
                mKeyCode = e.m_Ability.GetKeyCode();
                duration = e.m_Ability.GetAbilityCooldown();
            }

            switch (mKeyCode) {
                case KeyCode.Mouse0:
                    mAbilityCooldowns.Add(new AbilityCooldown() {
                        MTotalDuration = duration,
                        MCurrentTime = duration,
                        MImage = lmbImage
                    });
                    break;
                case KeyCode.Mouse1:
                    mAbilityCooldowns.Add(new AbilityCooldown() {
                        MTotalDuration = duration,
                        MCurrentTime = duration,
                        MImage = rmbImage
                    });
                    break;
                case KeyCode.Q:
                    mAbilityCooldowns.Add(new AbilityCooldown() {
                        MTotalDuration = duration,
                        MCurrentTime = duration,
                        MImage = mQ
                    });
                    break;
                case KeyCode.W:
                    mAbilityCooldowns.Add(new AbilityCooldown() {
                        MTotalDuration = duration,
                        MCurrentTime = duration,
                        MImage = mW
                    });
                    break;
                case KeyCode.E:
                    mAbilityCooldowns.Add(new AbilityCooldown() {
                        MTotalDuration = duration,
                        MCurrentTime = duration,
                        MImage = mE
                    });
                    break;
                case KeyCode.R:
                    mAbilityCooldowns.Add(new AbilityCooldown() {
                        MTotalDuration = duration,
                        MCurrentTime = duration,
                        MImage = mR
                    });
                    break;
            }
        }

        private void Update() {
            for (int i = 0; i < mAbilityCooldowns.Count; i++) {
                AbilityCooldown abilityCooldown = mAbilityCooldowns[i];
                abilityCooldown.MCurrentTime -= Time.deltaTime;
                float cooldownPercentage = abilityCooldown.MCurrentTime / abilityCooldown.MTotalDuration;
                abilityCooldown.MImage.fillAmount = cooldownPercentage;
                if (abilityCooldown.MCurrentTime <= 0) {
                    mAbilityCooldowns.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}