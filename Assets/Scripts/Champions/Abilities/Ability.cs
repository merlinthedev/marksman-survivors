﻿using System;
using System.Collections.Generic;
using System.Linq;
using Champions.Abilities.Upgrades;
using EventBus;
using TMPro.EditorUtilities;
using UnityEngine;

namespace Champions.Abilities {
    public abstract class Ability : MonoBehaviour, IUpgradeable, ICooldown {
        [SerializeField] private float abilityCooldown; // Static cooldown in seconds, should not be edited
        [SerializeField] protected float abilityRange = 999f; // Range of the ability in units, will change later on
        [SerializeField] private Sprite abilityLevelUpBanner;
        [SerializeField] private List<Upgrade> upgrades = new();
        private float currentCooldown;
        protected Champion champion;

        protected bool isCancelled = false;
        [SerializeField] protected float resourceCost = 0f;
        public AbilityUseType abilityUseType;
        public AbilityType abilityType;

        public bool ShouldTick => currentCooldown > 0;

        public virtual void Hook(Champion champion) {
            this.champion = champion;

            currentCooldown = 0;

            // Subscribe to the cooldown manager
            Subscribe(this);
        }

        private void OnDestroy() {
            Unsubscribe(this);
        }

        public void Tick(float deltaTime) {
            // Log("Ticking cooldown for " + this, Logger.Color.RED, this);
            currentCooldown -= deltaTime;
            // Log("Cooldown ticked! Current cooldown: " + currentCooldown, Logger.Color.RED, this);
        }

        public void Subscribe(ICooldown cooldown) {
            EventBus<SubscribeICooldownEvent>.Raise(new SubscribeICooldownEvent(cooldown));
        }

        public void Unsubscribe(ICooldown cooldown) {
            EventBus<UnsubscribeICooldownEvent>.Raise(new UnsubscribeICooldownEvent(cooldown));
        }

        public event Action OnCooldownCompleted;

        public virtual void OnUse() {
            // Log("An ability was used!", Logger.Color.RED, this);
            // Log("CurrentCooldown: " + currentCooldown + ", AbilityCooldown: " + abilityCooldown, this);
            currentCooldown = abilityCooldown;
            champion.GetChampionStatistics().CurrentMana -= resourceCost; // TODO: REFACTOR

            //Raise cooldown event
            EventBus<ChampionAbilityUsedEvent>.Raise(new ChampionAbilityUsedEvent(this));

            // Log("ShouldTick: " + ShouldTick + ", currentCooldown: " + currentCooldown, Logger.Color.RED, this);
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

        protected virtual void DeductFromCooldown(float timeToDeduct) {
            currentCooldown -= timeToDeduct;
        }

        protected bool IsOnCooldown() {
            // Debug.Log("we are on cooldown wtf are youdoing;  " + currentCooldown);
            return currentCooldown > 0;
        }

        protected bool CanAfford() {
            return champion.GetChampionStatistics().CurrentMana >= resourceCost;
        }

        public float GetAbilityCooldown() {
            return abilityCooldown;
        }

        public float GetCurrentCooldown() {
            return currentCooldown;
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

        public void SetCooldown(float v) {
            currentCooldown = v;
        }

        /// <summary>
        /// May need this in the future who knows
        /// </summary>
        public enum AbilityUseType {
            PASSIVE, // Passive abilities are always on
            ACTIVE, // Utility abilities are used to provide utility, like buffs or debuffs
        }

        public enum AbilityType {
            BASIC,
            UNIQUE_PASSIVE,
            OFFENSE,
            DEFENSE,
            MOBILITY,
            ULTIMATE
        }
    }
}