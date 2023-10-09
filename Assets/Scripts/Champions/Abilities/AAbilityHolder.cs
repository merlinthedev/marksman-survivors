using System;
using System.Collections.Generic;
using UnityEngine;

namespace Champions.Abilities {
    [Serializable]
    public class AAbilityHolder : MonoBehaviour {
        [SerializeField] protected List<AAbility> abilities = new List<AAbility>();

        public List<AAbility> GetAbilities() {
            return abilities;
        }
    }
}