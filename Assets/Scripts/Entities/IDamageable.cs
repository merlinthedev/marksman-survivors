using UnityEngine;

namespace Entities {
    public interface IDamageable {
        void TakeFlatDamage(float damage);
        Transform GetTransform();
    }
}