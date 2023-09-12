using System;
using UnityEngine;

namespace Champions.Kitegirl.Abilities {
    public class KitegirlR2 : AAbility {
        [SerializeField] private int m_AmountOfDeftnessStacks = 100;

        public override void OnUse() {
            base.OnUse();

            m_Champion.AddStacks(m_AmountOfDeftnessStacks, Stack.StackType.DEFTNESS);
        }
    }
}