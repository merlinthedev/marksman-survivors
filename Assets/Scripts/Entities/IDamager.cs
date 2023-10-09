namespace Entities {
    public interface IDamager {
        void DealDamage(IDamageable damageable, float damage);
        void SetCurrentTarget(IDamageable target);
        void ResetCurrentTarget();
    }
}