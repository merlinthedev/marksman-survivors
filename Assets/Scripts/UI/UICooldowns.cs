using System;
using System.Collections.Generic;
using Core;
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

        private Player player;

        private void Start() {
            player = Player.GetInstance();
            
            // for every image, set the fill amount to 1
            // this is because the cooldowns are not active at the start of the game
            lmbImage.fillAmount = 1;
            rmbImage.fillAmount = 1;
            qImage.fillAmount = 1;
            wImage.fillAmount = 1;
            eImage.fillAmount = 1;
            rImage.fillAmount = 1;
        }

        private void Update() {
            if (GameManager.GetInstance().Paused) return;
            foreach (var ability in player.GetCurrentlySelectedChampion().GetAbilities()) {
                switch (ability.GetKeyCode()) {
                    case KeyCode.Q:
                        qImage.fillAmount = ability.GetCurrentCooldown() / ability.GetAbilityCooldown();
                        break;
                    case KeyCode.W:
                        wImage.fillAmount = ability.GetCurrentCooldown() / ability.GetAbilityCooldown();
                        break;
                    case KeyCode.E:
                        eImage.fillAmount = ability.GetCurrentCooldown() / ability.GetAbilityCooldown();
                        break;
                    case KeyCode.R:
                        rImage.fillAmount = ability.GetCurrentCooldown() / ability.GetAbilityCooldown();
                        break;
                    case KeyCode.Mouse1:
                        rmbImage.fillAmount = ability.GetCurrentCooldown() / ability.GetAbilityCooldown();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            float x = Time.time;
            float y = player.GetCurrentlySelectedChampion().GetChampionStatistics().GetAttackSpeed(1);
            float z = player.GetCurrentlySelectedChampion().GetLastAttackTime();
            
            // lmbImage.fillAmount = (x - z) / y;
            // do the calculation above but flip the result
            lmbImage.fillAmount = 1 - (x - z) / y;
            
        }
    }
}