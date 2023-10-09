﻿using Champions.Abilities;
using UnityEngine;
using Util;

namespace Champions.Kitegirl.Abilities {
    public class KitegirlE1 : AAbility {
        private float dashDuration = 0.2f;
        private float lastDashTime = 0f;


        public override void OnUse() {
            if (IsOnCooldown()) return;
            
            Util.Logger.Log(" HELLO WIORLD " + Time.time, Util.Logger.Color.YELLOW, this);

            // Dash forward 
            (champion as Kitegirl)?.SetIsDashing(true);
            (champion as Kitegirl)?.SetNextAttackWillCrit(true);

            Utilities.InvokeDelayed(() => { (champion as Kitegirl)?.SetIsDashing(false); }, dashDuration,
                champion);

            base.OnUse();
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