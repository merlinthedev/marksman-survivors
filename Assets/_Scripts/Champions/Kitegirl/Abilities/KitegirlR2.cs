using BuffsDebuffs.Stacks;
using Champions.Abilities;
using UnityEngine;

namespace Champions.Kitegirl.Abilities {
    public class KitegirlR2 : Ability {
        [SerializeField] private int m_AmountOfDeftnessStacks = 100;

        public override void OnUse() {
            if (IsOnCooldown()) return;
            champion.AddStacks(m_AmountOfDeftnessStacks, Stack.StackType.DEFTNESS);
            base.OnUse();
        }
    }
}