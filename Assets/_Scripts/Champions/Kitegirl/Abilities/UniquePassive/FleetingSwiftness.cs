using _Scripts.BuffsDebuffs.Stacks;
using _Scripts.Champions.Abilities;
using _Scripts.Entities;
using UnityEngine;

namespace _Scripts.Champions.Kitegirl.Abilities.UniquePassive {
    public class FleetingSwiftness : Ability {
        [SerializeField] private float changeOfDeftness = 0.01f;
        [SerializeField] private int deftnessStacks = 1;

        public override void Hook(Champion champion) {
            base.Hook(champion);
            champion.OnBulletHit += Use;
        }

        private void Use(IDamageable damageable) {
            if (Random.Range(0f, 1f) <= changeOfDeftness) {
                this.champion.AddStacks(deftnessStacks, Stack.StackType.DEFTNESS);
            }
        }

        private void OnApplicationQuit() {
            this.champion.OnBulletHit -= Use;
        }
    }
}