namespace Entities {
    public interface IDamager {
        void DealDamage(IDamageable damageable, float damage, bool shouldInvoke = true);
        IDamageable currentTarget { get; set; }
        void ResetCurrentTarget();
    }
}