using Events;
using System;
using UnityEngine;

namespace Champions.Kitegirl.Abilities {
    [Serializable]
    public class KitegirlQ1 : AAbility {
        public override void OnUse() {
            if (IsOnCooldown()) return;
            base.OnUse();

            (this.m_Champion as global::Champions.Kitegirl.Kitegirl)?.SetAutoAttackChain(true);
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
}