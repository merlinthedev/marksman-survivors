using _Scripts.Champions;
using UnityEngine;

namespace _Scripts.Entities {
    public interface IDamager {
        GameObject gameObject { get; }
        void DealDamage(IDamageable damageable, float damage, Champion.DamageType damageType, bool shouldInvoke = true);
        IDamageable currentTarget { get; set; }
        void ResetCurrentTarget();
    }
}