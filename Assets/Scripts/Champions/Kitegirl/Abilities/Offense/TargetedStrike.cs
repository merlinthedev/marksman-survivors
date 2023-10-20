using Champions.Abilities;
using Champions.Kitegirl.Entities;
using Enemies;
using UnityEngine;

namespace Champions.Kitegirl.Abilities.Offense {
    public class TargetedStrike : Ability, ICastable {
        [field: SerializeField] public float CastTime { get; set; }
        [SerializeField] private KitegirlBullet bulletPrefab;
        [SerializeField] private float damagePercentage = 3f;

        private Enemy enemy;

        public override void Hook(Champion champion) {
            base.Hook(champion);
        }

        public override void OnUse() {
            if (!CanAfford()) {
                return;
            }

            if (Player.GetInstance().GetCurrentHoverEntity() == null) {
                return;
            }

            Cast();
        }

        private void use() {
            Vector3 direction = enemy.transform.position - champion.transform.position;
            direction.Normalize();

            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;


            KitegirlBullet bullet =
                Instantiate(bulletPrefab, champion.transform.position, Quaternion.Euler(0, angle, 0));
            
            
        }


        public void Cast() {
            enemy = (Enemy)Player.GetInstance().GetCurrentHoverEntity();

            Invoke(nameof(use), CastTime);
        }
    }
}