using _Scripts.Champions.Abilities;
using UnityEngine;

namespace _Scripts.Champions.Kitegirl.Abilities.Ultimate {
    public class BallisticFrenzy : Ability {
        [SerializeField] private int burstAmount = 3;
        

        public override void OnUse() {
            if (IsOnCooldown()) return;

            
            base.OnUse();
        }
    }
}