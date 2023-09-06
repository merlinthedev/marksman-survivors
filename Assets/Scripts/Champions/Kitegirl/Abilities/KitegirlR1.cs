using UnityEngine;
using Util;

namespace Champions.Kitegirl.Abilities {
    public class KitegirlR1 : AAbility {
        [SerializeField] private float m_UltimateDuration = 10f;
        [SerializeField] private float m_BurstAmount = 5f;
        [SerializeField] private float m_SlowAmount = 0.5f; // 50% slow, 0-1 Normalized!

        public override void OnUse() {
            if (IsOnCooldown()) return;

            (this.m_Champion as global::Champions.Kitegirl.Kitegirl)?.ActivateUltimate(m_UltimateDuration, m_BurstAmount, m_SlowAmount);
            m_LastUseTime = Time.time;

            Utilities.InvokeDelayed(() => { (this.m_Champion as global::Champions.Kitegirl.Kitegirl)?.DeactivateUltimate(); }, m_UltimateDuration,
                this.m_Champion);
        }
    }
}