using UnityEngine;

public class Stack {
    private StackType m_StackType;
    private float m_LifeTime = 10f;
    private float m_ApplyTime = 0f;
    private IStackableLivingEntity m_AffectedEntity;

    public Stack(StackType stackType, IStackableLivingEntity affectedEntity) {
        m_StackType = stackType;
        m_AffectedEntity = affectedEntity;

        m_ApplyTime = Time.time;
    }

    public void CheckForExpiration() {
        if (Time.time > m_ApplyTime + m_LifeTime) {
            m_AffectedEntity.RemoveStack(this);
        }
    }

    public StackType GetStackType() {
        return m_StackType;
    }

    public enum StackType {
        FRAGILE,
        DEFTNESS,
        OVERPOWER
    }
}