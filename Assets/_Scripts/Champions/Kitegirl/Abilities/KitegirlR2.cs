using _Scripts.BuffsDebuffs.Stacks;
using _Scripts.Champions.Abilities;
using UnityEngine;

namespace _Scripts.Champions.Kitegirl.Abilities {
    public class KitegirlR2 : Ability {
        [SerializeField] private int m_AmountOfDeftnessStacks = 100;

        public override void OnUse() {
            if (IsOnCooldown()) return;
            this.champion.AddStacks(m_AmountOfDeftnessStacks, Stack.StackType.DEFTNESS);
            base.OnUse();
        }
    }
}