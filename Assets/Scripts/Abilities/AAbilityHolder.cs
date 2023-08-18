using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AAbilityHolder : AEntity {
    [SerializeField] protected List<AAbility> m_Abilities = new List<AAbility>();
}