using _Scripts.Champions.Abilities;
using UnityEngine;

namespace _Scripts.Champions.Kitegirl.Abilities.UniquePassive {
    public class FlameOn : Ability {
        [SerializeField] private float flameChance = 0.1f;
        
        public override void Hook(Champion champion) {
            base.Hook(champion);
            champion.OnAbilityUsed += Use;
        }

        private void Use(Ability ability) {
            if (ability.abilityType == abilityType) {
                if (Random.Range(0f, 1f) <= flameChance) {
                    
                }
            }
        }
    }
}