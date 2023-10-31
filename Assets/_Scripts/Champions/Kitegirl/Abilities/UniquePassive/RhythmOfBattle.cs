using _Scripts.Champions.Abilities;
using UnityEngine;

namespace _Scripts.Champions.Kitegirl.Abilities.UniquePassive {
    public class RhythmOfBattle : Ability {
        [SerializeField] private float RhythmActivationTime = 5f;

        public override void Hook(Champion champion) {
            base.Hook(champion);

            champion.OnNonBasicAbilityDamageDone += Use;
            champion.RhythmActivationTime = RhythmActivationTime;
        }

        private void Use() {
            this.champion.LastNonBasicAbilityCastTime = Time.time;
        }

        private void OnApplicationQuit() {
            this.champion.OnNonBasicAbilityDamageDone -= Use;
        }
    }
}