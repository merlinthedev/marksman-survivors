using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AAbilityHolder : MonoBehaviour {
    [SerializeField] protected List<AAbility> m_Abilities = new List<AAbility>();

    public List<AAbility> GetAbilities() {
          return m_Abilities;
    }
}