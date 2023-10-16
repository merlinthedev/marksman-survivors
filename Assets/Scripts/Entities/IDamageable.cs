using UnityEngine;

namespace Entities {
    public interface IDamageable {
        void TakeFlatDamage(float damage);
        float CalculateIncomingDamage(float damage);
        Transform GetTransform();
    }
}