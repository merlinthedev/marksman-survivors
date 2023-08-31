using UnityEngine;

public abstract class AAbility : MonoBehaviour {

    [SerializeField] protected KeyCode m_KeyCode;
    [SerializeField] protected float m_AbilityCooldown = 0f;
    protected float m_LastUseTime;
    private float m_CurrentCooldown = 0f;
    protected Champion m_Champion;

    protected bool m_IsCancelled = false;
    

    public void Hook(Champion champion) {
        m_Champion = champion;

        m_LastUseTime = float.NegativeInfinity;

        // Debug.Log("Base Hook() called");
    }

    public abstract void OnUse();

    protected void SetBaseCooldown() {
        m_AbilityCooldown = m_CurrentCooldown;
    }

    protected virtual void ResetCooldown() {
        m_AbilityCooldown = 0f;
    }

    protected internal virtual void DeductFromCooldown(float timeToDeduct) {
        m_AbilityCooldown -= timeToDeduct;
    }

    public bool IsOnCooldown() {
        bool isOnCooldown = Time.time < m_LastUseTime + m_AbilityCooldown;
        return isOnCooldown;
    }

    public float GetAbilityCooldown() {
        return m_AbilityCooldown;
    }

    public float GetCurrentCooldown() {
        return Time.time - m_LastUseTime;
    }

    public KeyCode GetKeyCode() {
        return m_KeyCode;
    }


}