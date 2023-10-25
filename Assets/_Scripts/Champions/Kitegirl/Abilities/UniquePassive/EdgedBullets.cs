using Champions.Abilities;
using UnityEngine;

namespace Champions.Kitegirl.Abilities.UniquePassive {
    public class EdgedBullets : Ability {
        [SerializeField] private float critChanceIncrease = 0.05f;
        [SerializeField] private float critDamageIncrease = 0.05f;

        public override void Hook(Champion champion) {
            base.Hook(champion);

            champion.GetChampionStatistics().AddToStatistic(Statistic.CRITICAL_STRIKE_CHANCE, critChanceIncrease);
            champion.GetChampionStatistics().AddToStatistic(Statistic.CRITICAL_STRIKE_DAMAGE, critDamageIncrease);
        }
    }
}