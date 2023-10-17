using Entities;

namespace Enemies {
    public class Dummy : Enemy {
        protected override void Move() { }
        public override void DealDamage(IDamageable damageable, float damage, bool shouldInvoke = true) { }

        public override void TakeFlatDamage(float damage) {
            ShowDamageUI(damage);
        }
    }
}