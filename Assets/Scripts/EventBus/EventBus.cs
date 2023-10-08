using System;
using System.Collections.Generic;
using Champions.Abilities;
using UnityEngine;
using BuffsDebuffs.Stacks;
using Champions.Abilities.Upgrades;
using Core;
using Interactable.NPC;
using Inventory.Items;
using UnityEngine.UI;

namespace EventBus {
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

    public class EnemyStartHoverEvent : Event { }

    public class EnemyStopHoverEvent : Event { }

    public class InteractableStartHoverEvent : Event { }

    public class InteractableStopHoverEvent : Event { }

    public class EnemyHitEvent : Event {
        public Collider m_Collider { get; private set; }
        public Enemy.Enemy m_Enemy { get; private set; }
        public Vector3 m_BulletHitPosition { get; private set; }

        public EnemyHitEvent(Collider collider, Enemy.Enemy enemy, Vector3 bulletHitPosition) {
            m_Collider = collider;
            m_Enemy = enemy;
            m_BulletHitPosition = bulletHitPosition;
        }
    }

    public class EnemyKilledEvent : Event {
        public Collider Collider { get; private set; }
        public Enemy.Enemy Enemy { get; private set; }
        public Vector3 EnemyDeathPosition { get; private set; }

        public EnemyKilledEvent(Collider collider, Enemy.Enemy enemy, Vector3 enemyDeathPosition) {
            Collider = collider;
            Enemy = enemy;
            EnemyDeathPosition = enemyDeathPosition;
        }
    }

    public class ChampionHealthRegenerated : Event { }

    public class ChampionManaRegenerated : Event { }

    public class ChampionAbilitiesHookedEvent : Event { }

    public class ChampionDamageTakenEvent : Event { }

    public class ChampionAbilityUsedEvent : Event {
        public AAbility AbstractAbility { get; private set; }

        public KeyCode KeyCode { get; private set; }
        public float Duration { get; private set; }

        public ChampionAbilityUsedEvent(AAbility ability) {
            AbstractAbility = ability;
        }

        public ChampionAbilityUsedEvent(KeyCode keyCode, float duration) {
            KeyCode = keyCode;
            Duration = duration;
            AbstractAbility = null;
        }
    }

    public class ChampionLevelUpEvent : Event {
        public int m_CurrentLevel { get; private set; }
        public int m_PreviousLevel { get; private set; }
        public List<AAbility> m_ChampionAbilities { get; private set; }

        public ChampionLevelUpEvent(int currentLevel, int previousLevel, List<AAbility> championAbilities) {
            m_CurrentLevel = currentLevel;
            m_PreviousLevel = previousLevel;
            m_ChampionAbilities = championAbilities;
        }
    }

    public class ChampionAbilityChosenEvent : Event {
        public AAbility Ability { get; private set; }

        public ChampionAbilityChosenEvent(AAbility ability) {
            Ability = ability;
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