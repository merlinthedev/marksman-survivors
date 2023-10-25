using _Scripts.BuffsDebuffs.Stacks;
using _Scripts.Champions.Abilities;
using _Scripts.Champions.Effects;
using UnityEngine;

namespace _Scripts.Champions.Kitegirl.Abilities.Ultimate {
    public class Overdrive : Ability {
        [SerializeField] private int amountOfStacks = 10;
        [SerializeField] private Stack.StackType stackType = Stack.StackType.DEFTNESS;
        [SerializeField] private Effect effect;

        public override void Hook(Champion champion) {
            base.Hook(champion);
        }


        public override void OnUse() {
            base.OnUse();
        }
    }
}