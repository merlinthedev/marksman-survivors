using Champions.Abilities;

namespace Champions.Kitegirl.Abilities.Offense {
    public class DevastatingFlare : Ability {
        public override void Hook(Champion champion) {
            base.Hook(champion);
        }

        public override void OnUse() {
            if (!CanAfford()) {
                return;
            }
            
            
        }
    }
}