using Champions.Abilities;
using Util;

namespace Champions.Kitegirl.Abilities {
    public class KitegirlE1 : AAbility {
        private float dashDuration = 0.2f;
        private float lastDashTime = 0f;


        public override void OnUse() {
            if (IsOnCooldown()) return;

            // Util.Logger.Log(" HELLO WIORLD " + Time.time, Util.Logger.Color.YELLOW, this);

            // Dash forward 
            (champion as Kitegirl)?.SetIsDashing(true);
            (champion as Kitegirl)?.SetNextAttackWillCrit(true);
            float angle = Utilities.GetGlobalAngleFromDirection(champion);

            Utilities.InvokeDelayed(() => {
                (champion as Kitegirl)?.SetIsDashing(false);

                if (angle > 45) {
                    champion.Stop();
                    // Util.Logger.Log("Stopping the champion because the angle between our direction and our " +
                    //                 "position to the cursor is greater than 45", Util.Logger.Color.RED, this);
                }
            }, dashDuration, champion);

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