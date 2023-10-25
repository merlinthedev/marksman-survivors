using System;

namespace _Scripts.Entities {
    public interface IShieldable {
        float ShieldAmount { get; set; }
        float ShieldLifetime { get; set; }
        float LastShieldTime { get; set; }
        bool HasShield { get; }
        void SetShield(float amount);
        void AddShield(float amount);
        void SubtractShield(float amount);
        void RemoveShield();
        void CheckShieldExpiration();
        float TakeShieldDamage(float damage);
        event Action OnShieldExpired;

    }
}