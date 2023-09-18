using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Champions.Abilities {
    [Serializable]
    public class AAbilityHolder : MonoBehaviour {
        [SerializeField] protected List<AAbility> abilities = new List<AAbility>();

        public List<AAbility> GetAbilities() {
            return abilities;
        }
    }
}