using Champions.Abilities;
using Entities;
using UnityEngine;
using Util;

namespace Champions.Kitegirl.Abilities.Offense {
    public class DevastatingFlare : Ability, IChannelable {
        [SerializeField] private float projectileSpeed;
        [SerializeField] private float damagePercentage = 1f;
        [SerializeField] private Projectile projectilePrefab;
        public float ChannelTime { get; set; }

        private Vector3 direction;
        private Vector3 target;
        private float angle;


        public override void Hook(Champion champion) {
            base.Hook(champion);

            ChannelTime = 0.5f * champion.GetAttackSpeed();
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

            Channel();
        }

        private void use() {
            champion.SetIsChanneling(false);


            Vector3 pos = new Vector3(champion.transform.position.x, 0.16f, champion.transform.position.z);

            Projectile projectile = Instantiate(projectilePrefab, pos, Quaternion.Euler(0, angle, 0));
            projectile.Init(champion, target, OnHit);

            base.OnUse();
        }

        private void OnHit(IDamageable damageable) {
            champion.DealDamage(damageable, champion.GetAttackDamage() * damagePercentage,
                Champion.DamageType.NON_BASIC);
        }

        public void Channel() {
            (champion as Kitegirl)?.GetAnimator().SetDirection(angle);
            champion.SetGlobalDirectionAngle(angle);
            champion.Stop();
            champion.SetIsChanneling(true);

            Invoke(nameof(use), ChannelTime);
        }
    }
}