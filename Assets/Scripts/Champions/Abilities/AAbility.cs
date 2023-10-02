﻿using System;
using System.Collections.Generic;
using System.Linq;
using Champions.Abilities.Upgrades;
using EventBus;
using UnityEngine;
using Logger = Util.Logger;
using static Util.Logger;

namespace Champions.Abilities {
    public abstract class AAbility : MonoBehaviour, IUpgradeable, ICooldown {
        [SerializeField] protected KeyCode keyCode;
        [SerializeField] private float abilityCooldown = 0f; // Static cooldown in seconds, should not be edited
        [SerializeField] protected float abilityRange = 10f;
        [SerializeField] private Sprite abilityLevelUpBanner;
        [SerializeField] private List<Upgrade> upgrades = new();
        private float currentCooldown = 0f;
        protected Champion champion;

        protected bool isCancelled = false;

        public bool ShouldTick => currentCooldown > 0;

        public void Hook(Champion champion) {
            this.champion = champion;

            // Subscribe to the cooldown manager
            Subscribe(this);
        }

        private void OnDestroy() {
            Unsubscribe(this);
        }

        public void Tick(float deltaTime) {
            currentCooldown -= deltaTime;
        }

        public void Subscribe(ICooldown cooldown) {
            EventBus<SubscribeICooldownEvent>.Raise(new SubscribeICooldownEvent(cooldown, OnCooldownCompleted));
        }

        public void Unsubscribe(ICooldown cooldown) {
            EventBus<UnsubscribeICooldownEvent>.Raise(new UnsubscribeICooldownEvent(cooldown, OnCooldownCompleted));
        }

        public event Action OnCooldownCompleted;

        public virtual void OnUse() {
            Log("An ability was used!", Logger.Color.RED, this);
            currentCooldown = abilityCooldown;

            //Raise cooldown event
            EventBus<ChampionAbilityUsedEvent>.Raise(new ChampionAbilityUsedEvent(this));
        }

        public void OnUpgrade(Upgrade upgrade) {
            upgrade.OnApply();
        }

        protected bool DistanceCheck(Vector3 point) {
            return (champion.transform.position - point).magnitude <= abilityRange;
        }

        protected virtual void ResetCooldown() {
            currentCooldown = 0f;
        }

        protected internal virtual void DeductFromCooldown(float timeToDeduct) {
            currentCooldown -= timeToDeduct;
        }

        public bool IsOnCooldown() {
            return currentCooldown > 0;
        }

        public float GetAbilityCooldown() {
            return abilityCooldown;
        }

        public float GetCurrentCooldown() {
            return currentCooldown;
        }

        public KeyCode GetKeyCode() {
            return keyCode;
        }

        public Sprite GetAbilityLevelUpBannerSprite() {
            return abilityLevelUpBanner;
        }

        public List<Upgrade> GetUpgrades() {
            return upgrades;
        }

        public Upgrade GetNextUpgrade() {
            return upgrades.FirstOrDefault(upgrade => !upgrade.IsUnlocked());
        }

        /// <summary>
        /// May need this in the future who knows
        /// </summary>
        public enum AbilityType {
            PASSIVE, // Passive abilities are always on
            DAMAGE, // Damage abilities are used to deal damage
            MOBILITY, // Mobility abilities are used to move around
            UTILITY, // Utility abilities are used to provide utility, like buffs or debuffs
        }
    }
}