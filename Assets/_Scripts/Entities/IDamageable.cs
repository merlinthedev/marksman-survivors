using _Scripts.BuffsDebuffs.Stacks;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Entities {
    public interface IDamageable : IEntity {
        List<IAttachable> attachables { get; }
        void TakeFlatDamage(float damage);
        float CalculateIncomingDamage(float damage);
        Transform GetTransform();
    }
}