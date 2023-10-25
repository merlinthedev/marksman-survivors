using BuffsDebuffs.Stacks;
using Champions.Abilities;
using UnityEngine;
using Util;

namespace Champions.Kitegirl.Abilities {
    public class KitegirlQ3 : Ability {
        [SerializeField] private float activeTime = 2f;
        [SerializeField] private int stacksToAdd = 1;
        [SerializeField] private Stack.StackType stackType = Stack.StackType.DEFTNESS;

        public override void OnUse() {
            if (IsOnCooldown()) return;

            (champion as Kitegirl)?.SetAttackDeftnessApply(true);

            Utilities.InvokeDelayed(() => (champion as Kitegirl)?.SetAttackDeftnessApply(false), activeTime,
                champion);

            base.OnUse();
        }
    }
}