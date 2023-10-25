using UnityEngine;

namespace Champions.Effects.impl {
    public class MovementSpeedEffect : Effect {

        [SerializeField] private float movementSpeedModifier = 1.01f;

        public override void OnApply() {
            this.champion.GetChampionStatistics().MultiplyStatisticByPercentage(Statistic.MOVEMENT_SPEED, movementSpeedModifier);
            Debug.Log("Applied movementspeed effect.");
        }

        public override void OnExpire() {
            this.champion.GetChampionStatistics().DivideStatisticByPercentage(Statistic.MOVEMENT_SPEED, movementSpeedModifier);
            Debug.Log("Removed movementspeed effect.");

        }
    }
}