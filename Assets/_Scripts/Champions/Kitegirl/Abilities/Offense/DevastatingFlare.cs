using _Scripts.Champions.Abilities;
using _Scripts.Entities;
using _Scripts.Util;
using UnityEngine;

namespace _Scripts.Champions.Kitegirl.Abilities.Offense {
    public class DevastatingFlare : Ability, ICastable {
        [SerializeField] private SerializedDictionary<string, float> enemies = new();
        [SerializeField] private float projectileSpeed;
        [SerializeField] private float damagePercentage = 1f;
        [SerializeField] private Projectile projectilePrefab;
        public float CastTime { get; set; }

        private Vector3 direction;
        private Vector3 target;
        private float angle;


        public override void Hook(Champion champion) {
            base.Hook(champion);

            CastTime = 2f * champion.GetAttackSpeed();
        }

        public override void OnUse() {
            if (!CanAfford()) {
                return;
            }

            Vector3 mousePos = Utilities.GetMouseWorldPosition();
            direction = (mousePos - this.champion.transform.position).normalized;

            target = this.champion.transform.position + direction * this.abilityRange;
            target.y = 0.16f;

            angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;


            Cast();
        }

        private void Use() {
            this.champion.SetIsCasting(false);


            Vector3 pos = new Vector3(this.champion.transform.position.x, 0.16f, this.champion.transform.position.z);

            Projectile projectile = Instantiate(projectilePrefab, pos, Quaternion.Euler(0, angle, 0));
            projectile.Init(this.champion, target, OnHit, projectileSpeed, this.abilityRange);

        }

        private void OnHit(IDamageable damageable) {
            this.champion.DealDamage(damageable, this.champion.GetAttackDamage() * damagePercentage,
                Champion.DamageType.NON_BASIC);
        }

        public void Cast() {
            base.OnUse();

            (this.champion as Kitegirl)?.GetAnimator().SetDirection(angle);
            this.champion.SetGlobalDirectionAngle(angle);
            this.champion.Stop();
            this.champion.SetIsCasting(true);

            Invoke(nameof(Use), CastTime);
        }
    }
}