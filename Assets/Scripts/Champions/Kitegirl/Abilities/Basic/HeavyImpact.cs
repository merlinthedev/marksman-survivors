using Champions.Abilities;
using Core;
using Entities;
using UnityEngine;

namespace Champions.Kitegirl.Abilities.Basic {
    public class HeavyImpact : Ability {
        [SerializeField] private float damagePercentage = 0.33f;
        [SerializeField] private float coneAngle = 20f;
        [SerializeField] private float coneRange = 5f;

        public override void Hook(Champion champion) {
            base.Hook(champion);
            champion.OnBulletHit += Use;
        }

        private void Use(IDamageable damageable) {
            Vector3 damageablePosition = damageable.GetTransform().position;
            Vector3 championShootDirection = champion.GetLastAttackDirection();

            // get the direction into where we should find our triangle points
            Vector3 rightDirection = Quaternion.Euler(0, -coneAngle, 0) * championShootDirection;
            Vector3 leftDirection = Quaternion.Euler(0, coneAngle, 0) * championShootDirection;

            // our triangle points are 30 units away from the player
            Vector3 rightPoint = damageablePosition + rightDirection * coneRange;
            Vector3 leftPoint = damageablePosition + leftDirection * coneRange;

            // debug lines to visualize the triangle
            Debug.DrawLine(damageablePosition, rightPoint, Color.yellow, 0.5f);
            Debug.DrawLine(damageablePosition, leftPoint, Color.yellow, 0.5f);
            Debug.DrawLine(leftPoint, rightPoint, Color.yellow, 0.5f);

            var damageablesInCone = DamageableManager.GetInstance()
                .GetDamageablesInCone(damageablePosition, leftPoint, rightPoint, damageable);

            damageablesInCone.ForEach(d => { champion.DealDamage(d, champion.GetAttackDamage() * damagePercentage); });

            Debug.Log("x " + damageablesInCone.Count);
        }

        private void OnApplicationQuit() {
            champion.OnBulletHit -= Use;
        }
    }
}