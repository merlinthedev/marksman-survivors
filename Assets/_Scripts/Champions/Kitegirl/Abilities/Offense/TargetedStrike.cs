using _Scripts.Champions.Abilities;
using _Scripts.Enemies;
using _Scripts.Entities;
using UnityEngine;

namespace _Scripts.Champions.Kitegirl.Abilities.Offense {
    public class TargetedStrike : Ability, ICastable {
        [field: SerializeField] public float CastTime { get; set; }
        [SerializeField] private TargetProjectile bulletPrefab;
        [SerializeField] private float damagePercentage = 3f;
        [SerializeField] private float projectileSpeed = 30f;

        private Enemy enemy;
        private float angle = 0f;

        public override void OnUse() {
            if (!CanAfford()) {
                return;
            }

            if (Player.GetInstance().GetCurrentHoverEntity() == null) {
                return;
            }

            Cast();
        }

        private void Use() {
            this.champion.SetIsCasting(false);

            Vector3 direction = enemy.transform.position - this.champion.transform.position;
            direction.Normalize();

            angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;


            TargetProjectile targetProjectile = Instantiate(bulletPrefab, this.champion.transform.position, Quaternion.Euler(90, angle, 0));

            targetProjectile.Init(this.champion, enemy, OnHit, projectileSpeed);

        }


        private void OnHit(IDamageable damageable) {
            float dam = this.champion.GetAttackDamage() * damagePercentage;
            Debug.Log("Damage: " + dam);
            Debug.Log("AD: " + this.champion.GetAttackDamage());
            Debug.Log("Percentage: " + damagePercentage);
            this.champion.DealDamage(enemy, dam, Champion.DamageType.NON_BASIC);
        }


        public void Cast() {
            (this.champion as Kitegirl)?.GetAnimator().SetDirection(angle);
            this.champion.SetGlobalDirectionAngle(angle);
            this.champion.Stop();
            this.champion.SetIsCasting(true);

            enemy = (Enemy)Player.GetInstance().GetCurrentHoverEntity();

            if (enemy != null) {
                Invoke(nameof(Use), CastTime);
            }
        }
    }
}