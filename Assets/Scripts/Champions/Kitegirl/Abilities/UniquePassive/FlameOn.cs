using Champions.Abilities;
using UnityEngine;

namespace Champions.Kitegirl.Abilities.UniquePassive {
    public class FlameOn : Ability {
        [SerializeField] private float flameChance = 0.1f;
        [SerializeField] private AbilityType abilityType;
    }
}