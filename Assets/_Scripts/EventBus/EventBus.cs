﻿using _Scripts.BuffsDebuffs.Stacks;
using _Scripts.Champions.Abilities;
using _Scripts.Champions.Abilities.Upgrades;
using _Scripts.Core;
using _Scripts.Enemies;
using _Scripts.Interactable.NPC;
using _Scripts.Inventory.Items;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.EventBus {
    public abstract class EventBus<T> where T : Event {
        private static event Action<T> onEventRaised;

        public static void Subscribe(Action<T> action) {
            onEventRaised += action;
        }

        public static void Unsubscribe(Action<T> action) {
            onEventRaised -= action;
        }

        public static void Raise(T eventToRaise) {
            onEventRaised?.Invoke(eventToRaise);
        }
    }

    public abstract class Event { }

    public class EnemyStartHoverEvent : Event {
        public Enemy enemy;

        public EnemyStartHoverEvent(Enemy enemy) {
            this.enemy = enemy;
        }
    }


    public class EnemyStopHoverEvent : Event { }

    public class InteractableStartHoverEvent : Event { }

    public class InteractableStopHoverEvent : Event { }

    public class EnemySpawnedEvent : Event {
        public Enemy enemy { get; private set; }

        public EnemySpawnedEvent(Enemy enemy) {
            this.enemy = enemy;
        }
    }

    public class EnemyHitEvent : Event {
        public Collider m_Collider { get; private set; }
        public Enemy m_Enemy { get; private set; }
        public Vector3 m_BulletHitPosition { get; private set; }

        public EnemyHitEvent(Collider collider, Enemy enemy, Vector3 bulletHitPosition) {
            m_Collider = collider;
            m_Enemy = enemy;
            m_BulletHitPosition = bulletHitPosition;
        }
    }

    public class EnemyKilledEvent : Event {
        public Collider collider { get; private set; }
        public Enemy enemy { get; private set; }
        public Vector3 enemyDeathPosition { get; private set; }

        public EnemyKilledEvent(Collider collider, Enemy enemy, Vector3 enemyDeathPosition) {
            this.collider = collider;
            this.enemy = enemy;
            this.enemyDeathPosition = enemyDeathPosition;
        }
    }

    public class ChampionHealthRegenerated : Event { }

    public class ChampionManaRegenerated : Event { }

    public class ChampionAbilitiesHookedEvent : Event { }

    public class ChampionDamageTakenEvent : Event { }

    public class ChampionAbilityUsedEvent : Event {
        public Ability AbstractAbility { get; private set; }

        public KeyCode KeyCode { get; private set; }
        public float Duration { get; private set; }

        public ChampionAbilityUsedEvent(Ability ability) {
            AbstractAbility = ability;
        }

        public ChampionAbilityUsedEvent(KeyCode keyCode, float duration) {
            KeyCode = keyCode;
            Duration = duration;
            AbstractAbility = null;
        }
    }

    public class ChampionLevelUpEvent : Event {
        public int CurrentLevel { get; private set; }
        public int PreviousLevel { get; private set; }
        public List<Ability> ChampionAbilities { get; private set; }

        public ChampionLevelUpEvent(int currentLevel, int previousLevel, List<Ability> championAbilities) {
            CurrentLevel = currentLevel;
            PreviousLevel = previousLevel;
            ChampionAbilities = championAbilities;
        }
    }

    public class ChampionAbilityChosenEvent : Event {
        public Ability Ability { get; private set; }
        public bool ShouldAdd { get; private set; }

        public ChampionAbilityChosenEvent(Ability ability, bool shouldAdd) {
            Ability = ability;
            ShouldAdd = shouldAdd;
        }
    }

    public class ChampionUpgradeChosenEvent : Event {
        public Upgrade Upgrade { get; private set; }

        public ChampionUpgradeChosenEvent(Upgrade upgrade) {
            Upgrade = upgrade;
        }
    }

    public class MerchantInteractEvent : Event {
        public List<Item> items { get; private set; } = new();
        public Merchant merchant { get; private set; }

        public MerchantInteractEvent(List<Item> items, Merchant merchant) {
            this.items = items;
            this.merchant = merchant;
        }
    }

    public class MerchantItemBuyRequestEvent : Event {
        public Item item { get; private set; }
        public Button panelButton;

        public MerchantItemBuyRequestEvent(Item item) {
            this.item = item;
        }
    }

    public class MerchantExitEvent : Event {
        public Merchant merchant { get; private set; }

        public MerchantExitEvent(Merchant merchant) {
            this.merchant = merchant;
        }
    }

    public class AddGoldEvent : Event {
        public int AmountToAdd { get; private set; }

        public AddGoldEvent(int amountToAdd) {
            AmountToAdd = amountToAdd;
        }
    }

    public class UpdateResourceBarEvent : Event {
        public string type { get; private set; }
        public float current { get; private set; }
        public float total { get; private set; }

        public UpdateResourceBarEvent(string type, float current, float total) {
            this.type = type;
            this.current = current;
            this.total = total;
        }
    }

    public class ChangeStackUIEvent : Event {
        public Stack.StackType type;
        public int stacks;
        public bool open;

        public ChangeStackUIEvent(Stack.StackType type, int stacks, bool open) {
            this.type = type;
            this.stacks = stacks;
            this.open = open;
        }
    }

    public class UIGoldChangedEvent : Event {
        public int Gold { get; private set; }

        public UIGoldChangedEvent(int gold) {
            this.Gold = gold;
        }
    }

    public class UIKillCounterChangedEvent : Event {
        public int KillCount { get; private set; }

        public UIKillCounterChangedEvent(int killCount) {
            KillCount = killCount;
        }
    }

    public class UILevelUpPanelClosedEvent : Event { }

    public class UILevelUpPanelOpenEvent : Event { }

    public class ShowLevelUpPanelEvent : Event { }

    public class LevelUpPromptEvent : Event {
        public bool open;

        public LevelUpPromptEvent(bool open) {
            this.open = open;
        }
    }


    public class StartDialogueEvent : Event {
        //List of titles and bodies for dialogue that the DialogueManager will copy
        public List<Dialogue> dialogue { get; private set; }

        //In case the NPC needs to do something during  or after the dialogue, you may pass the NPC gameobject.
        //You may also pass "null" in case this is not relevant.
        public GameObject npc { get; private set; }

        public StartDialogueEvent(List<Dialogue> dialogue, GameObject npc) {
            this.dialogue = dialogue;
            this.npc = npc;
        }
    }

    public class LoadSceneEvent : Event {
        public string sceneName { get; private set; }

        public LoadSceneEvent(string sceneName) {
            this.sceneName = sceneName;
        }
    }

    public class SubscribeICooldownEvent : Event {
        public ICooldown Cooldown { get; private set; }

        public SubscribeICooldownEvent(ICooldown cooldown) {
            Cooldown = cooldown;
        }
    }

    public class UnsubscribeICooldownEvent : Event {
        public ICooldown Cooldown { get; private set; }

        public UnsubscribeICooldownEvent(ICooldown cooldown) {
            Cooldown = cooldown;
        }
    }

    public class GamePausedEvent : Event { }

    public class GameResumedEvent : Event { }

    public class ToggleMenuEvent : Event {
        public string menu { get; private set; }

        public ToggleMenuEvent(string menuName) {
            this.menu = menuName;
        }
    }

    public class UISettingsMenuOpenedEvent : Event { }

    public class UISettingsMenuClosedEvent : Event { }
}