using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Champions.Abilities {
    [Serializable]
    public class AbilityHolder : MonoBehaviour {
        [SerializeField] protected List<Ability> abilities = new List<Ability>();

        public List<Ability> GetAbilities() {
            return abilities;
        }
    }
}