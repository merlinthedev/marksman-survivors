using System;
using System.Collections.Generic;
using _Scripts.EventBus;
using _Scripts.Util;
using UnityEngine;

namespace _Scripts.Champions.Abilities {
    [Serializable]
    public class AbilityHolder : MonoBehaviour {
        [SerializeField] protected List<Ability> abilities = new();
        [SerializeField] protected List<Ability> passiveAbilities = new();

        public List<Ability> GetAbilities() {
            return abilities;
        }

        public List<Ability> GetPassives() {
            return passiveAbilities;
        }

        protected void Start() {
            Debug.Log("Start");
            Utilities.InvokeNextFrame(
                () => {
                    abilities.ForEach(ability => {
                        EventBus<ChampionAbilityChosenEvent>.Raise(new ChampionAbilityChosenEvent(ability, false));
                    });
                }, this);
        }
    }
}