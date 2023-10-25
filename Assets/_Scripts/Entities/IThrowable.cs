using _Scripts.BuffsDebuffs.Stacks;
using UnityEngine;

namespace _Scripts.Entities {
    public interface IThrowable {
        public void OnThrow(Vector3 targetPoint, IEntity sourceEntity);
    }
}