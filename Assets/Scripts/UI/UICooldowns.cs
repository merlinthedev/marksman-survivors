using System.Collections.Generic;
using EventBus;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class UICooldowns : MonoBehaviour {
        [Header("References")]
        [SerializeField] private Image lmbImage;

        [SerializeField] private Image rmbImage;
        [SerializeField] private Image qImage;
        [SerializeField] private Image wImage;
        [SerializeField] private Image eImage;
        [SerializeField] private Image rImage;

        private List<AbilityCooldown> abilityCooldowns = new();

        private class AbilityCooldown {
            public float TotalDuration;
            public float CurrentTime;
            public Image Image;
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
            qImage.fillAmount = 1;
            wImage.fillAmount = 1;
            eImage.fillAmount = 1;
            rImage.fillAmount = 1;
        }

        private void HandleAbilityCooldowns(ChampionAbilityUsedEvent e) {
            KeyCode keyCode;
            float duration;

            if (e.AbstractAbility == null) {
                keyCode = e.KeyCode;
                duration = e.Duration;
            }
            else {
                keyCode = e.AbstractAbility.GetKeyCode();
                duration = e.AbstractAbility.GetAbilityCooldown();
            }

            switch (keyCode) {
                case KeyCode.Mouse0:
                    abilityCooldowns.Add(new AbilityCooldown() {
                        TotalDuration = duration,
                        CurrentTime = duration,
                        Image = lmbImage
                    });
                    break;
                case KeyCode.Mouse1:
                    abilityCooldowns.Add(new AbilityCooldown() {
                        TotalDuration = duration,
                        CurrentTime = duration,
                        Image = rmbImage
                    });
                    break;
                case KeyCode.Q:
                    abilityCooldowns.Add(new AbilityCooldown() {
                        TotalDuration = duration,
                        CurrentTime = duration,
                        Image = qImage
                    });
                    break;
                case KeyCode.W:
                    abilityCooldowns.Add(new AbilityCooldown() {
                        TotalDuration = duration,
                        CurrentTime = duration,
                        Image = wImage
                    });
                    break;
                case KeyCode.E:
                    abilityCooldowns.Add(new AbilityCooldown() {
                        TotalDuration = duration,
                        CurrentTime = duration,
                        Image = eImage
                    });
                    break;
                case KeyCode.R:
                    abilityCooldowns.Add(new AbilityCooldown() {
                        TotalDuration = duration,
                        CurrentTime = duration,
                        Image = rImage
                    });
                    break;
            }
        }

        private void Update() {
            for (int i = 0; i < abilityCooldowns.Count; i++) {
                AbilityCooldown abilityCooldown = abilityCooldowns[i];
                abilityCooldown.CurrentTime -= Time.deltaTime;
                float cooldownPercentage = abilityCooldown.CurrentTime / abilityCooldown.TotalDuration;
                abilityCooldown.Image.fillAmount = cooldownPercentage;
                if (abilityCooldown.CurrentTime <= 0) {
                    abilityCooldowns.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}