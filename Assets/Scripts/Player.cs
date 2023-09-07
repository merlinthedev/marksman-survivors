﻿using Events;
using System;
using Champions;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

public class Player : MonoBehaviour {
    [Header("Stats")]
    [SerializeField] private Texture2D m_CursorTexture, m_AttackCursorTexture;

    [SerializeField] private LayerMask m_AttackLayerMask;

    [Header("UI")]
    [SerializeField] private Image m_HealthBar;

    [SerializeField] private Champion m_SelectedChampion;

    [Header("Other")]
    [SerializeField] private GameObject m_ClickAnimPrefab;

    private void OnEnable() {
        EventBus<EnemyStartHoverEvent>.Subscribe(OnEnemyStartHover);
        EventBus<EnemyStopHoverEvent>.Subscribe(OnEnemyStopHover);
        EventBus<ChampionHealthRegenerated>.Subscribe(OnChampionHealthRegenerated);
        EventBus<ChampionManaRegenerated>.Subscribe(OnChampionManaRegenerated);
        EventBus<ChampionAbilitiesHookedEvent>.Subscribe(OnChampionAbilitiesHooked);
        EventBus<ChampionDamageTakenEvent>.Subscribe(OnChampionDamageTakenEvent);
    }

    private void OnDisable() {
        EventBus<EnemyStartHoverEvent>.Unsubscribe(OnEnemyStartHover);
        EventBus<EnemyStopHoverEvent>.Unsubscribe(OnEnemyStopHover);
        EventBus<ChampionHealthRegenerated>.Unsubscribe(OnChampionHealthRegenerated);
        EventBus<ChampionManaRegenerated>.Unsubscribe(OnChampionManaRegenerated);
        EventBus<ChampionAbilitiesHookedEvent>.Unsubscribe(OnChampionAbilitiesHooked);
        EventBus<ChampionDamageTakenEvent>.Unsubscribe(OnChampionDamageTakenEvent);
    }

    private void Start() {
        UpdateHealthBar();
        SetDefaultCursorTexture();
    }

    private bool m_FirstMove = true;

    // Deprecated
    [Obsolete("Use HandleMouseClicks() instead")]
    private void HandleMoveClick() {
        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.gameObject.CompareTag("Ground")) {
                    var point = hit.point;
                    point.y = transform.position.y;
                    // m_HitPoint = point;

                    if (m_FirstMove) {
                        m_FirstMove = false;
                        // EnemyManager.GetInstance().SetShouldSpawn(true);
                        // Debug.Log("Start enemy spawning");
                    }


                    m_SelectedChampion.SetMouseHitPoint(point);
                }
            }
        }
    }

    [Obsolete("Use HandleMouseClicks() instead")]
    private void HandleAttackClick() {
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.A)) {
            m_SelectedChampion.OnAutoAttack(null);
        }
    }

    private void HandleMouseClicks() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            int layerMask = LayerMask.GetMask("ExcludeFromMovementClicks");
            layerMask = ~layerMask;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
                // Debug.Log("<color=yellow>Hit: " + hit.collider.gameObject.name + "</color>");
                if (hit.collider.gameObject.CompareTag("Ground")) {
                    var point = hit.point;
                    Instantiate(m_ClickAnimPrefab, new Vector3(point.x, 0.2f, point.z), Quaternion.identity);
                    point.y = transform.position.y;
                    // m_HitPoint = point;

                    if (m_FirstMove) {
                        m_FirstMove = false;
                        // EnemyManager.GetInstance().SetShouldSpawn(true);
                        // Debug.Log("Start enemy spawning");
                    }


                    m_SelectedChampion.SetMouseHitPoint(point);
                }

                if (hit.collider.gameObject.CompareTag("Enemy") ||
                    hit.collider.gameObject.CompareTag("KitegirlGrenade")) {
                    m_SelectedChampion.OnAutoAttack(hit.collider);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            m_SelectedChampion.OnAbility(KeyCode.Mouse1);
        }
    }

    private void HandleAbilityClicks() {
        // If Q, W, E or R is pressed, call the m_SelectedChampion.OnAbility() method and pass in the correct KeyCode

        if (Input.GetKeyDown(KeyCode.Q)) {
            m_SelectedChampion.OnAbility(KeyCode.Q);
        }

        if (Input.GetKeyDown(KeyCode.W)) {
            m_SelectedChampion.OnAbility(KeyCode.W);
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            m_SelectedChampion.OnAbility(KeyCode.E);
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            m_SelectedChampion.OnAbility(KeyCode.R);
        }
    }

    private void Update() {
        HandleMouseClicks();
        HandleAbilityClicks();
    }


    private void UpdateHealthBar() {
        float healthPercentage = m_SelectedChampion.GetCurrentHealth() / m_SelectedChampion.GetMaxHealth();
        // Debug.Log("Health percentage: " + healthPercentage + "", this);

        m_HealthBar.fillAmount = healthPercentage;
    }

    private void SetDefaultCursorTexture() {
        Cursor.SetCursor(m_CursorTexture, Vector2.zero, CursorMode.Auto);
    }

    private void OnEnemyStartHover(EnemyStartHoverEvent e) {
        Cursor.SetCursor(m_AttackCursorTexture, Vector2.zero, CursorMode.Auto);
    }

    private void OnEnemyStopHover(EnemyStopHoverEvent e) {
        SetDefaultCursorTexture();
    }

    private void OnChampionDamageTakenEvent(ChampionDamageTakenEvent e) {
        EventBus<UpdateResourceBarEvent>.Raise(new UpdateResourceBarEvent("Health",
            m_SelectedChampion.GetCurrentHealth(), m_SelectedChampion.GetMaxHealth()));
    }

    private void OnChampionHealthRegenerated(ChampionHealthRegenerated e) {
        EventBus<UpdateResourceBarEvent>.Raise(new UpdateResourceBarEvent("Health",
            m_SelectedChampion.GetCurrentHealth(), m_SelectedChampion.GetMaxHealth()));
    }

    private void OnChampionManaRegenerated(ChampionManaRegenerated e) {
        // TODO: update the mana bar
    }

    private void OnChampionAbilitiesHooked(ChampionAbilitiesHookedEvent e) {
        //UpdateQCooldown();
        //UpdateWCooldown();
        //UpdateECooldown();
        //UpdateRCooldown();
    }

    public Champion GetCurrentlySelectedChampion() {
        return m_SelectedChampion;
    }
}