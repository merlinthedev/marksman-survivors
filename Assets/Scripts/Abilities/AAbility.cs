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

        m_LastUseTime = 0f;

        Debug.Log("Base Hook() called");
    }

    public abstract void OnUse();

    protected void SetBaseCooldown() {
        m_AbilityCooldown = m_CurrentCooldown;
    }

    protected virtual void ResetCooldown() {
        m_AbilityCooldown = 0f;
    }

    protected virtual void DeductFromCooldown(float timeToDeduct) {
        m_AbilityCooldown -= timeToDeduct;
    }

    protected bool IsOnCooldown() {
        bool isOnCooldown = Time.time < m_LastUseTime + m_AbilityCooldown;
        Debug.Log("Is on cooldown: " + isOnCooldown);
        return isOnCooldown;
    }

    public KeyCode GetKeyCode() {
        return m_KeyCode;
    }


}