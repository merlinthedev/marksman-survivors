using System;
using BuffsDebuffs.Stacks;
using Champions.Abilities;
using UnityEngine;

namespace Champions.Kitegirl.Abilities {
    public class KitegirlQ3 : AAbility {
        [SerializeField] private float m_ActiveTime = 2f;
        [SerializeField] private int m_StacksToAdd = 1;
        [SerializeField] private Stack.StackType m_StackType = Stack.StackType.DEFTNESS; 
        
        public override void OnUse() {
            
            
            base.OnUse();
        }
    }
}