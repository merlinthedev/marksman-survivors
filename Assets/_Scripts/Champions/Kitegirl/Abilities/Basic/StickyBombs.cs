using _Scripts.Champions.Abilities;
using _Scripts.Entities;
using UnityEngine;

namespace _Scripts.Champions.Kitegirl.Abilities.Basic {
    public class StickyBombs : Ability {
        [SerializeField] private StickyBomb stickyBombPrefab;
        [SerializeField] private float timeToExplode = 3f;
        [SerializeField] private float damagePercentage = 0.1f;
        [SerializeField] private float damageArea = 5f;

        public override void Hook(Champion champion) {
            base.Hook(champion);
            champion.OnBulletHit += Use;
        }

        private void Use(IDamageable damageable) {
            StickyBomb stickyBomb =
                Instantiate(stickyBombPrefab, damageable.GetTransform().position, Quaternion.identity);
            stickyBomb.Init(timeToExplode, damageArea, damagePercentage);
            stickyBomb.OnAttach(damageable, this.champion);
        }

        private void OnApplicationQuit() {
            this.champion.OnBulletHit -= Use;
        }
    }
}