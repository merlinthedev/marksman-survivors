using System;

[Serializable]
public class KitegirlQ : AAbility {
    public override void OnUse() {
        // TODO: MAKE THE ABILITY WORK

        (this.m_Champion as Kitegirl)?.SetAutoAttackChain(true);
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