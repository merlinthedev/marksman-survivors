using System;
using UnityEngine;

namespace BuffsDebuffs.Stacks {
    public class Stack {
        private StackType stackType;
        private float lifeTime = 10f;
        private float applyTime = 0f;
        private IStackableLivingEntity affectedEntity;

        public Stack(StackType stackType, IStackableLivingEntity affectedEntity) {
            this.stackType = stackType;
            this.affectedEntity = affectedEntity;

            applyTime = Time.time;
        }

        public void CheckForExpiration() {
            if (Time.time > applyTime + lifeTime) {
                affectedEntity.RemoveStack(this);
            }
        }

        public StackType GetStackType() {
            return stackType;
        }

        [Serializable]
        public enum StackType {
            defaultStack,
            FRAGILE,
            DEFTNESS,
            OVERPOWER,
            FOCUS
        }
    }
}