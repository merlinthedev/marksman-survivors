using _Scripts.Champions.Abilities;
using _Scripts.Entities;
using UnityEngine;

namespace _Scripts.Champions.Kitegirl.Abilities.Basic {
    public class BouncingBullets : Ability {
        [SerializeField] private float timeBetweenBounces = 0.3f;
        [SerializeField] private int bounces = 2;

        public override void Hook(Champion champion) {
            base.Hook(champion);
            champion.OnBulletHit += Use;
        }

        private void Use(IDamageable damageable) {
            (this.champion as Kitegirl)?.Bounce(bounces, timeBetweenBounces, damageable);
        }
        

        private void OnApplicationQuit() {
            this.champion.OnBulletHit -= Use;
        }
        
        

        // WE NEED THIS FUNCTION DO NOT DELETE
        protected override void ResetCooldown() {
            base.ResetCooldown();
        }

        // WE NEED THIS FUNCTION DO NOT DELETE
        protected override void DeductFromCooldown(float timeToDeduct) {
            base.DeductFromCooldown(timeToDeduct);
        }
    }
}