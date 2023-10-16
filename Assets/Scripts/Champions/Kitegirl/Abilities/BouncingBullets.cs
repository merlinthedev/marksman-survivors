using Champions.Abilities;

namespace Champions.Kitegirl.Abilities {
    public class BouncingBullets : AAbility {
        public override void OnUse() {
            if (IsOnCooldown()) return;

            (champion as Kitegirl)?.SetAutoAttackChain(true);

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