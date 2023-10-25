using _Scripts.BuffsDebuffs.Stacks;
using _Scripts.Champions.Abilities;
using _Scripts.Util;
using UnityEngine;

namespace _Scripts.Champions.Kitegirl.Abilities {
    public class KitegirlQ3 : Ability {
        [SerializeField] private float activeTime = 2f;
        [SerializeField] private int stacksToAdd = 1;
        [SerializeField] private Stack.StackType stackType = Stack.StackType.DEFTNESS;

        public override void OnUse() {
            if (IsOnCooldown()) return;

            (this.champion as Kitegirl)?.SetAttackDeftnessApply(true);

            Utilities.InvokeDelayed(() => (this.champion as Kitegirl)?.SetAttackDeftnessApply(false), activeTime,
                this.champion);

            base.OnUse();
        }
    }
}