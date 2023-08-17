using System;
using UnityEngine;

[Serializable]
public class KitegirlQ : AAbility {
    public override void OnUse() {
        // TODO: MAKE THE ABILITY WORK
        if (IsOnCooldown()) {
            return;
        }

        (this.m_Champion as Kitegirl)?.SetAutoAttackChain(true);
        m_LastUseTime = Time.time;

        Debug.Log("Kitegirl Q used");

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