using UnityEngine;
using Util;

public class KitegirlR : AAbility {

    [SerializeField] private float m_UltimateDuration = 12f;

    public override void OnUse() {
        if (IsOnCooldown()) return;

        (this.m_Champion as Kitegirl)?.ActivateUltimate();

        Utilities.InvokeDelayed(() => {
            (this.m_Champion as Kitegirl)?.DeactivateUltimate();
        }, m_UltimateDuration, this.m_Champion);
    }
}