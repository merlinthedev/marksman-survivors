using _Scripts.Champions.Abilities;
using UnityEngine;

namespace _Scripts.Champions.Kitegirl.Abilities.Defense {
    public class Resourceful : Ability {
        [SerializeField] [Tooltip("Normalized! 0-1")] private float percentageToRegenerate = 0.5f;

        public override void OnUse() {
            if (IsOnCooldown()) {
                Debug.Log("Ability is on cooldown");
                return;
            }

            Debug.Log("Adding...");
            this.champion.GetChampionStatistics().AddToStatistic(Statistic.CURRENT_MANA, this.champion.GetChampionStatistics().MaxMana * percentageToRegenerate);

            base.OnUse();
        }
    }
}