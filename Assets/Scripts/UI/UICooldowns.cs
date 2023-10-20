using System;
using Champions;
using System.Collections.Generic;
using Champions.Abilities;
using Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class UICooldowns : MonoBehaviour {
        [Header("References")]
        [SerializeField] private Image lmbCooldown;
        [SerializeField] private GameObject lmbDisable;
        [SerializeField] private Image rmbCooldown;
        [SerializeField] private GameObject rmbDisable;
        [SerializeField] private Image qCooldown;
        [SerializeField] private GameObject qDisable;
        [SerializeField] private Image wCooldown;
        [SerializeField] private GameObject wDisable;
        [SerializeField] private Image eCooldown;
        [SerializeField] private GameObject eDisable;
        [SerializeField] private Image rCooldown;
        [SerializeField] private GameObject rDisable;
        [SerializeField] private Image spaceCooldown;
        [SerializeField] private GameObject spaceDisable;

        private Player player;

        private void Start() {
            player = Player.GetInstance();
            
            // for every image, set the fill amount to 0
            // this is because the cooldowns are not active at the start of the game
            lmbCooldown.fillAmount = 0;
            lmbDisable.SetActive(false);
            rmbCooldown.fillAmount = 0;
            rmbDisable.SetActive(true);
            qCooldown.fillAmount = 0;
            qDisable.SetActive(true);
            wCooldown.fillAmount = 0;
            wDisable.SetActive(true);
            eCooldown.fillAmount = 0;
            eDisable.SetActive(true);
            rCooldown.fillAmount = 0;
            rDisable.SetActive(true);
            spaceCooldown.fillAmount = 0;
            spaceDisable.SetActive(true);
        }

        private void Update() {
            if (GameManager.GetInstance().Paused) return;
            foreach (var ability in player.GetCurrentlySelectedChampion().GetAbilities()) {
                // switch (ability.GetKeyCode()) {
                //     case KeyCode.Q:
                //         if(!qDisable.activeSelf) qDisable.SetActive(true);
                //         qCooldown.fillAmount = ability.GetCurrentCooldown() / ability.GetAbilityCooldown();
                //         if(qCooldown.fillAmount == 0) qDisable.SetActive(false);
                //         break;
                //     case KeyCode.W:
                //         if(!wDisable.activeSelf) wDisable.SetActive(true);
                //         wCooldown.fillAmount = ability.GetCurrentCooldown() / ability.GetAbilityCooldown();
                //         if(wCooldown.fillAmount == 0) wDisable.SetActive(false);
                //         break;
                //     case KeyCode.E:
                //         if(!eDisable.activeSelf) eDisable.SetActive(true);
                //         eCooldown.fillAmount = ability.GetCurrentCooldown() / ability.GetAbilityCooldown();
                //         if(eCooldown.fillAmount == 0) eDisable.SetActive(false);
                //         break;
                //     case KeyCode.R:
                //         if(!rDisable.activeSelf) rDisable.SetActive(true);
                //         rCooldown.fillAmount = ability.GetCurrentCooldown() / ability.GetAbilityCooldown();
                //         if(rCooldown.fillAmount == 0) rDisable.SetActive(false);
                //         break;
                //     case KeyCode.Mouse1:
                //         if(!rmbDisable.activeSelf) rmbDisable.SetActive(true);
                //         rmbCooldown.fillAmount = ability.GetCurrentCooldown() / ability.GetAbilityCooldown();
                //         if(rmbCooldown.fillAmount == 0) rmbDisable.SetActive(false);
                //         break;
                //     default:
                //         throw new ArgumentOutOfRangeException();
                // }
            }

        }
    }
}