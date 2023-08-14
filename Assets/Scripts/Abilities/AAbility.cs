using UnityEngine;

public abstract class AAbility : MonoBehaviour {

    [SerializeField] protected KeyCode m_KeyCode;
    protected float m_AbilityCooldown = 0f;
    protected Champion m_Champion;

    public void Hook(Champion champion) {
        m_Champion = champion;
    }

    public abstract void OnUse();

    protected virtual void ResetCooldown() {
        m_AbilityCooldown = 0f;
    }

    protected virtual void DeductFromCooldown(float timeToDeduct) {
        m_AbilityCooldown -= timeToDeduct;
    }

    public KeyCode GetKeyCode() {
        return m_KeyCode;
    }
}