using UnityEngine;

namespace _Scripts.Champions.Effects.impl {
    public class AttackSpeedEffect : Effect {
        [SerializeField] private float attackSpeedMultiplier = 1.1f;

        public override void OnApply() {
            this.champion.GetChampionStatistics().MultiplyStatisticByPercentage(Statistic.ATTACK_SPEED, attackSpeedMultiplier);
        }

        public override void OnExpire() {
            this.champion.GetChampionStatistics().DivideStatisticByPercentage(Statistic.ATTACK_SPEED, attackSpeedMultiplier);
        }

        public float GetAttackSpeedMultiplier() {
            return attackSpeedMultiplier;
        }
    }
}