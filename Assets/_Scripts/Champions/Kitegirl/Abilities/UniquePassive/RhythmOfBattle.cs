using _Scripts.Champions.Abilities;
using UnityEngine;

namespace _Scripts.Champions.Kitegirl.Abilities.UniquePassive {
    public class RhythmOfBattle : Ability {
        [SerializeField] private float RhythmActivationTime = 5f;

        public override void Hook(Champion champion) {
            base.Hook(champion);

            champion.OnAbilityUsed += Use;
            champion.RhythmActivationTime = RhythmActivationTime;
        }

        private void Use(Ability ability) {
            if (ability.abilityType == AbilityType.BASIC) return;

            this.champion.LastNonBasicAbilityCastTime = Time.time;
        }

        private void OnApplicationQuit() {
            this.champion.OnAbilityUsed -= Use;
        }
    }
}