﻿using _Scripts.BuffsDebuffs.Stacks;
using _Scripts.Champions.Abilities;
using _Scripts.Champions.Effects;
using _Scripts.Champions.Effects.impl;
using UnityEngine;

namespace _Scripts.Champions.Kitegirl.Abilities.Defense {
    public class Focus : Ability {
        [SerializeField] private int stacksToAdd = 1;
        [SerializeField] private Effect effect;

        public override void Hook(Champion champion) {
            effect.Init(champion);
            
            base.Hook(champion);
        }

        public override void OnUse() {
            if (IsOnCooldown()) {
                Debug.Log("Focus is on cooldown.");
                return;
            }

            this.champion.AddStacks(stacksToAdd, Stack.StackType.FOCUS, effect);
            effect.OnApply();
            this.champion.OnDamageTaken += Use;

            base.OnUse();
        }

        private void Use() {
            this.champion.GetChampionStatistics().DivideStatisticByPercentage(Statistic.ATTACK_SPEED, (effect as AttackSpeedEffect).GetAttackSpeedMultiplier());

            this.champion.OnDamageTaken -= Use;
        }
    }
}