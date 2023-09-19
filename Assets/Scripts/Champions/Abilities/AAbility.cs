using System.Collections.Generic;
using EventBus;
using UnityEngine;
using Logger = Util.Logger;

namespace Champions.Abilities {
    public abstract class AAbility : MonoBehaviour {
        [SerializeField] protected KeyCode keyCode;
        [SerializeField] protected float abilityCooldown = 0f;
        [SerializeField] protected float abilityRange = 10f;
        protected float lastUseTime;
        private float currentCooldown = 0f;
        protected Champion champion;


        protected bool isCancelled = false;


        public void Hook(Champion champion) {
            this.champion = champion;

            lastUseTime = float.NegativeInfinity;

            // Debug.Log("Base Hook() called");
        }

        public virtual void OnUse() {
            Logger.Log("An ability was used!", Logger.Color.RED, this);
            lastUseTime = Time.time;

            //Raise cooldown event
            EventBus<ChampionAbilityUsedEvent>.Raise(new ChampionAbilityUsedEvent(this));
        }

        protected void SetBaseCooldown() {
            abilityCooldown = currentCooldown;
        }

        protected bool DistanceCheck(Vector3 point) {
            return (this.champion.transform.position - point).magnitude <= abilityRange;
        }

        protected virtual void ResetCooldown() {
            abilityCooldown = 0f;
        }

        protected internal virtual void DeductFromCooldown(float timeToDeduct) {
            lastUseTime -= timeToDeduct;
        }

        public bool IsOnCooldown() {
            bool isOnCooldown = Time.time < lastUseTime + abilityCooldown;
            return isOnCooldown;
        }

        public float GetAbilityCooldown() {
            return abilityCooldown;
        }

        public float GetCurrentCooldown() {
            return Time.time - lastUseTime;
        }

        public KeyCode GetKeyCode() {
            return keyCode;
        }
    }
}