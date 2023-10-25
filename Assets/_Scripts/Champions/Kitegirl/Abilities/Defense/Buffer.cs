using _Scripts.Champions.Abilities;
using _Scripts.Champions.Effects;
using UnityEngine;

namespace _Scripts.Champions.Kitegirl.Abilities.Defense {
    public class Buffer : Ability {
        [SerializeField] private float shieldAmount = 1f;
        [SerializeField] private float shieldLifetime = 30f;
        [SerializeField] private Effect effect;


        public override void Hook(Champion champion) {
            effect.Init(champion);

            base.Hook(champion);
        }

        public override void OnUse() {
            if (IsOnCooldown()) {
                // Debug.Log("Buffer is on cooldown.");
                return;
            }

            this.champion.SetShield(shieldAmount);
            this.champion.ShieldLifetime = shieldLifetime;
            effect.OnApply();

            this.champion.OnShieldExpired += OnShieldExpiration;


            base.OnUse();
        }

        private void OnShieldExpiration() {
            // Debug.Log("Shield expired.");
            effect.OnExpire();
            this.champion.OnShieldExpired -= OnShieldExpiration;
        }


    }
}