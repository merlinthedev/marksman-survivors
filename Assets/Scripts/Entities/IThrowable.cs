using BuffsDebuffs.Stacks;
using UnityEngine;

namespace Entities {
    public interface IThrowable {
        public void OnThrow(Vector3 targetPoint, IEntity sourceEntity);
    }
}