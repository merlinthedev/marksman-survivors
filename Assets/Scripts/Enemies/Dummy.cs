using Champions;
using Entities;

namespace Enemies {
    public class Dummy : Enemy {
        protected override void Move() { }

        public override void DealDamage(IDamageable damageable, float damage, Champion.DamageType damageType,
            bool shouldInvoke = true) { }

        public override void TakeFlatDamage(float damage) {
            ShowDamageUI(damage);
        }
    }
}