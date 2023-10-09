namespace Entities {
    public interface IDamager {
        void DealDamage(IDamageable damageable, float damage);
        IDamageable currentTarget { get; set; }
        void ResetCurrentTarget();
    }
}