using UnityEngine;

public class KitegirlW : AAbility {

    private float m_GrenadeThrowTime = 0f;
    private float m_DetonateTime = 2f;


    public override void OnUse() {
        if (IsOnCooldown()) {
            return;
        }
    }

    // WE NEED THIS FUNCTION DO NOT DELETE
    protected override void ResetCooldown() {
        base.ResetCooldown();

    }

    // WE NEED THIS FUNCTION DO NOT DELETE
    protected override void DeductFromCooldown(float timeToDeduct) {
        base.DeductFromCooldown(timeToDeduct);

    }
}