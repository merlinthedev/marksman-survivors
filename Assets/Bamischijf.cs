using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BuffsDebuffs.Stacks;
using Stack = BuffsDebuffs.Stacks.Stack;
using Core;
using Enemy;
using static BuffsDebuffs.Stacks.Stack;
using Util;
using Champions.Abilities;
using EventBus;
using UnityEngine.UI;

public class Bamischijf : MonoBehaviour {
    bool enemiesShouldSpawn = true;
    bool cooldownsShouldReset = false;
    [SerializeField] private Image panel;

    private void OnEnable() {
        EventBus<ChampionAbilityUsedEvent>.Subscribe(ResetCooldowns);
    }

    private void OnDisable() {
        EventBus<ChampionAbilityUsedEvent>.Unsubscribe(ResetCooldowns);
    }

    public void CloseMenu() {
        if (!enemiesShouldSpawn) {
            Utilities.InvokeDelayed(() => EnemyManager.GetInstance().SetShouldSpawn(enemiesShouldSpawn), 0.1f,
                EnemyManager.GetInstance().GetComponent<EnemyManager>());
        }

        panel.gameObject.SetActive(false);
    }

    public StackType StringToEnum(string x) {
        switch (x) {
            case "overpower":
                return StackType.OVERPOWER;
            case "fragile":
                return StackType.FRAGILE;
            case "deftness":
                return StackType.DEFTNESS;
            default:
                return StackType.defaultStack;
        }
    }

    public void GiveStacks(string x) {
        StackType stack = StringToEnum(x);
        Player.GetInstance().GetCurrentlySelectedChampion().AddStacks(10, stack);
    }

    public void GiveGold() {
        Player.GetInstance().GetInventory().AddGold(999);
    }

    public void SpawnMerchant() {
        Vector3 location = Player.GetInstance().GetCurrentlySelectedChampion().transform.position;
        location.x += 5;
        EventManager.GetInstance().InstantiateMerchant(location);
    }

    public void ToggleEnemySpawn() {
        enemiesShouldSpawn = !enemiesShouldSpawn;
        Util.Logger.Log("enemies should spawn: " + enemiesShouldSpawn, Util.Logger.Color.YELLOW, this);
        EnemyManager.GetInstance().SetShouldSpawn(!EnemyManager.GetInstance().GetShouldSpawn());
    }

    public void SpawnEnemy(int x) {
        EnemyManager.GetInstance().InstantiateEnemies(EnemyManager.GetInstance().FindPositionsIteratively(x));
    }

    public void SpawnDummy() {
        Vector3[] location = new Vector3[1];
        location[0] = Player.GetInstance().GetCurrentlySelectedChampion().transform.position + new Vector3(5, 0, 0);
        Enemy.Enemy[] enemy = EnemyManager.GetInstance().InstantiateEnemies(location);
        enemy[0].MakeDummy();
    }

    public void WipeEnemies() {
        EnemyManager.GetInstance().WipeEnemies();
    }

    public void ChangeTimescale(int x) {
        if (x == 0) {
            Time.timeScale = 1;
        } else {
            Time.timeScale += x;
            Debug.Log("timescale: " + Time.timeScale);
        }
    }

    public void Suicide() {
        Player.GetInstance().GetCurrentlySelectedChampion().OnDeath();
    }

    public void ToggleInvincibility() {
        Player.GetInstance().GetCurrentlySelectedChampion().ToggleInvincibility();
    }

    public void LevelUp() {
        Player.GetInstance().GetCurrentlySelectedChampion().GetChampionStatistics().CurrentXP += 100;
        Player.GetInstance().GetCurrentlySelectedChampion().GetChampionLevelManager().CheckForLevelUp();
    }

    public void ToggleResetCooldowns() {
        cooldownsShouldReset = !cooldownsShouldReset;
    }

    private void ResetCooldowns(ChampionAbilityUsedEvent e) {
        if (!cooldownsShouldReset) return;
        List<AAbility> abilities = Player.GetInstance().GetCurrentlySelectedChampion().GetAbilities();
        for (int i = 0; i < abilities.Count; i++) {
            abilities[i].currentCooldown = 0;
        }
    }
}