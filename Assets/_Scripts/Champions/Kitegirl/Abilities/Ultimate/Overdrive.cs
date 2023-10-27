using _Scripts.BuffsDebuffs.Stacks;
using _Scripts.Champions.Abilities;
using _Scripts.Champions.Effects;
using _Scripts.Champions.Effects.impl;
using UnityEngine;

namespace _Scripts.Champions.Kitegirl.Abilities.Ultimate {
    public class Overdrive : Ability {
        [SerializeField] private int amountOfStacks = 10;
        [SerializeField] private Stack.StackType stackType = Stack.StackType.DEFTNESS;
        [SerializeField] private Effect effect;
        [SerializeField] private float initialResourcePercentageGain = 0.5f;

        public override void Hook(Champion champion) {
            effect.Init(champion);
            (effect as OverdriveEffect).Link(this);
            base.Hook(champion);
        }


        public override void OnUse() {
            if (IsOnCooldown()) {
                Debug.Log("Overdrive is on cooldown.");
                return;
            }

            champion.AddStacks(amountOfStacks, stackType);
            this.champion.GetChampionStatistics().TryAddMana(this.champion.GetChampionStatistics().MaxMana * initialResourcePercentageGain);

            effect.OnApply();
            base.OnUse();
        }

        public void OnInterval(int stacks) {
            this.champion.AddStacks(stacks, stackType);
        }
    }
}