using BuffsDebuffs.Stacks;
using Champions.Abilities;
using Entities;
using UnityEngine;

namespace Champions.Kitegirl.Abilities.UniquePassive {
    public class FleetingSwiftness : Ability {
        [SerializeField] private float changeOfDeftness = 0.01f;
        [SerializeField] private int deftnessStacks = 1;

        public override void Hook(Champion champion) {
            base.Hook(champion);
            champion.OnBulletHit += Use;
        }

        private void Use(IDamageable damageable) {
            if (Random.Range(0f, 1f) <= changeOfDeftness) {
                champion.AddStacks(deftnessStacks, Stack.StackType.DEFTNESS);
            }
        }

        private void OnApplicationQuit() {
            champion.OnBulletHit -= Use;
        }
    }
}