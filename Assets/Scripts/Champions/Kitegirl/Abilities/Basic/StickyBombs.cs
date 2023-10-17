using System.Collections.Generic;
using Champions.Abilities;
using Core;
using Entities;
using UnityEngine;
using Util;

namespace Champions.Kitegirl.Abilities.Basic {
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
            stickyBomb.OnAttach(damageable, champion);
        }

        private void OnApplicationQuit() {
            champion.OnBulletHit -= Use;
        }
    }
}