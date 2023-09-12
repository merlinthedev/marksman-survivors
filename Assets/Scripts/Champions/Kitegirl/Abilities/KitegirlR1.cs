using Champions.Abilities;
using UnityEngine;
using Util;

namespace Champions.Kitegirl.Abilities {
    public class KitegirlR1 : AAbility {
        [SerializeField] private float m_UltimateDuration = 10f;
        [SerializeField] private float m_BurstAmount = 5f;
        [SerializeField] private float m_SlowAmount = 0.5f; // 50% slow, 0-1 Normalized!

        public override void OnUse() {
            if (IsOnCooldown()) return;

            (this.m_Champion as Kitegirl)?.ActivateUltimate(m_UltimateDuration, m_BurstAmount, m_SlowAmount);

            Utilities.InvokeDelayed(() => { (m_Champion as Kitegirl)?.DeactivateUltimate(); }, m_UltimateDuration,
                m_Champion);
            base.OnUse();
        }
    }
}