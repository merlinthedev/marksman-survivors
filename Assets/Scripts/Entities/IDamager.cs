using Champions;

namespace Entities {
    public interface IDamager {
        void DealDamage(IDamageable damageable, float damage, Champion.DamageType damageType, bool shouldInvoke = true);
        IDamageable currentTarget { get; set; }
        void ResetCurrentTarget();
    }
}