using _Scripts.Champions.Abilities;
using _Scripts.Util;
using UnityEngine;

namespace _Scripts.Champions.Kitegirl.Abilities.Ultimate {
    public class BallisticFrenzy : Ability {
        [SerializeField] private int burstAmount = 3;
        [SerializeField] private float ultimateDuration = 10f;

        public override void Hook(Champion champion) {
            base.Hook(champion);
            (champion as Kitegirl).maxBrust = burstAmount;
        }

        public override void OnUse() {
            if (IsOnCooldown()) return;

            (champion as Kitegirl).shouldPierce = true;
            (champion as Kitegirl).shouldBurst = true;

            Utilities.InvokeDelayed(Deactivate, ultimateDuration, this.champion);

            base.OnUse();
        }

        private void Deactivate() {
            (champion as Kitegirl).shouldPierce = false;
            (champion as Kitegirl).shouldBurst = false;
        }
    }
}