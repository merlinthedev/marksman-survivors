using System;
using UnityEngine;

[Serializable]
public class KitegirlQ1 : AAbility {
    public override void OnUse() {
        if (IsOnCooldown()) return;


        (this.m_Champion as Kitegirl)?.SetAutoAttackChain(true);
        m_LastUseTime = Time.time;


    }

    // WE NEED THIS FUNCTION DO NOT DELETE
    protected override void ResetCooldown() {
        base.ResetCooldown();

    }

    // WE NEED THIS FUNCTION DO NOT DELETE
    protected internal override void DeductFromCooldown(float timeToDeduct) {
        base.DeductFromCooldown(timeToDeduct);

    }
}