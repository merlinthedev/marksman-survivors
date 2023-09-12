using System;
using Champions.Abilities;
using UnityEngine;

namespace Champions.Kitegirl.Abilities {
    [Serializable]
    public class KitegirlQ1 : AAbility {
        public override void OnUse() {
            if (IsOnCooldown()) return;

            (m_Champion as Kitegirl)?.SetAutoAttackChain(true);

            base.OnUse();
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