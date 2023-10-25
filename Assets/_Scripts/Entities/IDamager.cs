using Champions;
using UnityEngine;

namespace Entities {
    public interface IDamager {
        GameObject gameObject { get; }
        void DealDamage(IDamageable damageable, float damage, Champion.DamageType damageType, bool shouldInvoke = true);
        IDamageable currentTarget { get; set; }
        void ResetCurrentTarget();
    }
}