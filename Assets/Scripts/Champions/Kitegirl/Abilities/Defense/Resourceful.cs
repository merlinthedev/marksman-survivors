using Champions.Abilities;
using UnityEngine;

namespace Champions.Kitegirl.Abilities.Defense {
    public class Resourceful : Ability {
        [SerializeField] [Tooltip("Normalized! 0-1")] private float percentageToRegenerate = 0.5f;

        public override void OnUse() {
            if (IsOnCooldown()) {
                Debug.Log("Ability is on cooldown");
                return;
            }

            Debug.Log("Adding...");
            champion.GetChampionStatistics().AddToStatistic(Statistic.CURRENT_MANA, this.champion.GetChampionStatistics().MaxMana * percentageToRegenerate);

            base.OnUse();
        }
    }
}