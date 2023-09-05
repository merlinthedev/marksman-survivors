using UnityEngine;
using Util;

public class KitegirlR1 : AAbility {
    [SerializeField] private float m_UltimateDuration = 12f;

    public override void OnUse() {
        if (IsOnCooldown()) return;

        (this.m_Champion as Kitegirl)?.ActivateUltimate(m_UltimateDuration);
        m_LastUseTime = Time.time;

        Utilities.InvokeDelayed(() => { (this.m_Champion as Kitegirl)?.DeactivateUltimate(); }, m_UltimateDuration,
            this.m_Champion);
    }
}