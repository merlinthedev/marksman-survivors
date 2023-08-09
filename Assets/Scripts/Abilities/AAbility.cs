using UnityEngine;

public abstract class AAbility : MonoBehaviour {

    protected float m_AbilityCooldown = 0f;

    public abstract void OnUse();
    public abstract void ResetCooldown();
    public abstract void DeductFromCooldown(float timeToDeduct);
}