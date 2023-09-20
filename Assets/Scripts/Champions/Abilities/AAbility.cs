using System.Collections.Generic;
using Champions.Abilities.Upgrades;
using EventBus;
using UnityEngine;
using Logger = Util.Logger;
using static Util.Logger;

namespace Champions.Abilities {
    public abstract class AAbility : MonoBehaviour, IUpgradeable {
        [SerializeField] protected KeyCode keyCode;
        [SerializeField] protected float abilityCooldown = 0f;
        [SerializeField] protected float abilityRange = 10f;
        [SerializeField] private Sprite abilityLevelUpBanner;
        [SerializeField] private List<Upgrade> Upgrades = new();
        private float lastUseTime;
        private float currentCooldown = 0f;
        protected Champion champion;

        protected bool isCancelled = false;


        public void Hook(Champion champion) {
            this.champion = champion;

            lastUseTime = float.NegativeInfinity;
        }

        public virtual void OnUse() {
            Log("An ability was used!", Logger.Color.RED, this);
            lastUseTime = Time.time;

            //Raise cooldown event
            EventBus<ChampionAbilityUsedEvent>.Raise(new ChampionAbilityUsedEvent(this));
        }

        public void OnUpgrade(Upgrade upgrade) {
            upgrade.OnApply();
        }

        protected void SetBaseCooldown() {
            abilityCooldown = currentCooldown;
        }

        protected bool DistanceCheck(Vector3 point) {
            return (champion.transform.position - point).magnitude <= abilityRange;
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

        public Sprite GetAbilityLevelUpBannerSprite() {
            return abilityLevelUpBanner;
        }

        public enum AbilityType {
            PASSIVE, // Passive abilities are always on
            DAMAGE, // Damage abilities are used to deal damage
            MOBILITY, // Mobility abilities are used to move around
            UTILITY, // Utility abilities are used to provide utility, like buffs or debuffs
        }
    }
}