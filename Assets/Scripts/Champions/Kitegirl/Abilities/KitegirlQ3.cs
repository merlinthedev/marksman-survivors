using BuffsDebuffs.Stacks;
using Champions.Abilities;
using UnityEngine;
using Util;

namespace Champions.Kitegirl.Abilities {
    public class KitegirlQ3 : AAbility {
        [SerializeField] private float m_ActiveTime = 2f;
        [SerializeField] private int m_StacksToAdd = 1;
        [SerializeField] private Stack.StackType m_StackType = Stack.StackType.DEFTNESS;

        public override void OnUse() {
            if (IsOnCooldown()) return;

            (champion as Kitegirl)?.SetAutoAttackDeftnessApply(true);

            Utilities.InvokeDelayed(() => (champion as Kitegirl)?.SetAutoAttackDeftnessApply(false), m_ActiveTime,
                champion);

            base.OnUse();
        }
    }
}