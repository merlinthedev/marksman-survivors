﻿using _Scripts.Champions.Abilities;
using _Scripts.Entities;
using _Scripts.Util;
using UnityEngine;

namespace _Scripts.Champions.Kitegirl.Abilities.Offense {
    public class DevastatingFlare : Ability, ICastable {
        [SerializeField] private SerializedDictionary<string, float> enemies = new();
        [SerializeField] private float projectileSpeed;
        [SerializeField] private float damagePercentage = 1f;
        [SerializeField] private float casttimeFactor = 2f;
        [SerializeField] private Projectile projectilePrefab;
        private bool piercing = true;
        private int maxPierce = -1;
        public float CastTime { get; set; }

        private Vector3 direction;
        private Vector3 target;
        private float angle;


        public override void Hook(Champion champion) {
            base.Hook(champion);

            CastTime = casttimeFactor * champion.GetAttackSpeed();
        }

        public override void OnUse() {
            if (!CanAfford()) {
                return;
            }

            Vector3 mousePos = Utilities.GetMouseWorldPosition();
            direction = (mousePos - champion.transform.position).normalized;

            target = champion.transform.position + direction * abilityRange;
            target.y = 0.16f;

            angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            Cast();
        }

        private void Use() {
            champion.SetIsCasting(false);


            Vector3 pos = new Vector3(champion.transform.position.x, 0.16f, champion.transform.position.z);

            Projectile projectile = Instantiate(projectilePrefab, pos, Quaternion.Euler(0, angle, 0));
            projectile.Init(champion, target, OnHit, projectileSpeed, abilityRange, piercing);

            base.OnUse();
        }

        private void OnHit(IDamageable damageable) {
            champion.DealDamage(damageable, champion.GetAttackDamage() * damagePercentage,
                Champion.DamageType.NON_BASIC);
        }

        public void Cast() {
            (champion as Kitegirl)?.GetAnimator().SetDirection(angle);
            champion.SetGlobalDirectionAngle(angle);
            champion.Stop();
            champion.SetIsCasting(true);

            Invoke(nameof(Use), CastTime);
        }
    }
}