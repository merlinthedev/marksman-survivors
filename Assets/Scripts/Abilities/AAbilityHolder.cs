using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AAbilityHolder : MonoBehaviour {
    [SerializeField] protected List<AAbility> m_Abilities = new List<AAbility>();

    private void Start() {
        Debug.Log("Ability holder start");
    }

}