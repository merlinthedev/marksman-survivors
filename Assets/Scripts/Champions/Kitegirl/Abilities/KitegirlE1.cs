﻿using UnityEngine;
using Util;

namespace Champions.Kitegirl.Abilities {
    public class KitegirlE1 : AAbility {
        private float m_DashDuration = 0.2f;
        private float m_LastDashTime = 0f;

        public override void OnUse() {
            if (IsOnCooldown()) return;

            // Dash forward 
            (this.m_Champion as global::Champions.Kitegirl.Kitegirl)?.SetIsDashing(true);
            (m_Champion as global::Champions.Kitegirl.Kitegirl)?.SetNextAttackWillCrit(true);
            m_LastUseTime = Time.time;

            Utilities.InvokeDelayed(() => { (this.m_Champion as global::Champions.Kitegirl.Kitegirl)?.SetIsDashing(false); }, m_DashDuration,
                this.m_Champion);
        }

        // WE NEED THIS FUNCTION DO NOT DELETE
        protected override void ResetCooldown() {
            base.ResetCooldown();
        }

        // WE NEED THIS FUNCTION DO NOT DELETE
        protected internal override void DeductFromCooldown(float timeToDeduct) {
            base.DeductFromCooldown(timeToDeduct);

            // Debug.Log("Deducted from cooldown.");
        }
    }
}