using Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Util;

public class Enemy : MonoBehaviour, IDamageable {
    private Transform m_Target;
    [SerializeField] private GameObject m_EnemyDamageNumberPrefab;
    private Canvas m_Canvas;

    private Rigidbody m_Rigidbody;
    private Collider m_Collider;
    [SerializeField] private float m_MovementSpeed = 7f;
    private float m_InitialMovementSpeed;


    private float m_MaxHealth = 250f;
    private float m_CurrentHealth = 250f;
    [SerializeField] private Image m_HealthBar;
    private float m_InitialHealthBarWidth;
    private bool m_CanMove = true;

    private float m_LastAttackTime = 0f;
    [SerializeField] private float m_AttackCooldown = 3f;

    [SerializeField] private float m_RewardXP = 1f;

    private SpriteRenderer m_SpriteRenderer;
    private Animator m_Animator;
    [SerializeField] private int m_CurrentDir = 0;
    private int m_NewDir = 0;
    [SerializeField] float Xspeed;
    [SerializeField] float Zspeed;

    private bool m_IsDead {
        get => !m_CanMove;
    }

    public bool IsBurning { get; set; }
    public bool IsFragile { get; set; }
    public float FragileStacks { get; set; }
    public float LastFragileApplyTime { get; set; }
    public List<Debuff> Debuffs { get; } = new();

    private void OnMouseEnter() {
        EventBus<EnemyStartHoverEvent>.Raise(new EnemyStartHoverEvent());
    }

    private void OnMouseExit() {
        EventBus<EnemyStopHoverEvent>.Raise(new EnemyStopHoverEvent());
    }

    private void Start() {
        m_Rigidbody = GetComponent<Rigidbody>();
        if (m_Rigidbody == null) {
            throw new Exception("Missing rigidbody");
        }

        m_Canvas = GetComponentInChildren<Canvas>();
        if (m_Canvas == null) {
            throw new Exception("Missing canvas");
        }

        m_Collider = GetComponent<Collider>();
        if (m_Collider == null) {
            throw new Exception("Missing collider");
        }

        m_InitialHealthBarWidth = m_HealthBar.rectTransform.sizeDelta.x;
        UpdateHealthBar();

        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Animator = GetComponent<Animator>();

        m_InitialMovementSpeed = m_MovementSpeed;
    }


    public void SetTarget(Transform target) {
        m_Target = target;
    }

    private void Update() {
        if (m_Target != null && m_CanMove) {
            Move();
        }

        //Check dir
        if (m_Rigidbody.velocity.x > 0) {
            m_CurrentDir = (m_Rigidbody.velocity.z > 0) ? 0 : 1;
        }
        else if (m_Rigidbody.velocity.x < 0) {
            m_CurrentDir = (m_Rigidbody.velocity.z < 0) ? 2 : 3;
        }


        //If dir changed, flip sprite
        if (m_CurrentDir != m_NewDir) {
            m_NewDir = m_CurrentDir;
            m_Animator.SetTrigger("DirChange");
            if (m_CurrentDir == 0) {
                m_SpriteRenderer.flipX = true;
                m_Animator.SetInteger("Dir", 1);
            }
            else if (m_CurrentDir == 1) {
                m_SpriteRenderer.flipX = true;
                m_Animator.SetInteger("Dir", 0);
            }
            else if (m_CurrentDir == 2) {
                m_SpriteRenderer.flipX = false;
                m_Animator.SetInteger("Dir", 0);
            }
            else if (m_CurrentDir == 3) {
                m_SpriteRenderer.flipX = false;
                m_Animator.SetInteger("Dir", 1);
            }
        }

        Xspeed = m_Rigidbody.velocity.x;
        Zspeed = m_Rigidbody.velocity.z;
    }

    private void Move() {
        Vector3 direction = m_Target.position - transform.position;

        if (direction.magnitude < 0.1f) {
            m_Target = null;
            return;
        }

        m_Rigidbody.velocity = direction.normalized * m_MovementSpeed;
    }


    public void TakeFlatDamage(float damage) {
        TakeDamage(damage);
    }

    public void TakeBurnDamage(float damage, float interval, float time) {
        IsBurning = true;
        StartCoroutine(BurnDamageCoroutine(damage, interval, time));
    }

    private IEnumerator BurnDamageCoroutine(float damage, float interval, float time) {
        float startTime = Time.time;
        while (Time.time < startTime + time) {
            TakeDamage(damage);
            yield return new WaitForSeconds(interval);
        }

        IsBurning = false;
    }

    private void TakeDamage(float damage) {
        if (m_IsDead) return;
        m_CurrentHealth -= damage;

        ShowDamageUI(damage);
        UpdateHealthBar();

        if (m_CurrentHealth <= 0) {
            m_CanMove = false;
            m_Rigidbody.velocity = Vector3.zero;

            Invoke(nameof(Die), 0.5f);
        }
    }

    public void AddFragileStacks(float stacks) {
        if (FragileStacks < 1) IsFragile = true;
        FragileStacks += stacks;
    }

    public void RemoveFragileStacks(float stacks) {
        if (FragileStacks < stacks) {
            FragileStacks = 0;
            IsFragile = false;
        }
        else {
            FragileStacks -= stacks;
        }
    }

    public void Die() {
        EventBus<EnemyKilledEvent>.Raise(new EnemyKilledEvent(m_Collider, this, transform.position));

        Destroy(gameObject);
    }

    public void ApplyDebuff(Debuff debuff) {
        switch (debuff.GetDebuffType()) {
            case Debuff.DebuffType.SLOW:
                ApplySlow(debuff);
                break;
        }
    }

    public void RemoveDebuff(Debuff debuff) {
        Debuffs.Remove(debuff);
        switch (debuff.GetDebuffType()) {
            case Debuff.DebuffType.SLOW:
                RemoveSlow(debuff);
                break;
        }
        // throw new NotImplementedException();
    }

    private void ApplySlow(Debuff debuff) {
        // m_MovementSpeed *= value;
        // The value is 0.33, how can i decrease the speed by 33%?
        m_MovementSpeed *= 1 - debuff.GetValue();

        if (debuff.GetDuration() < 0) {
            return;
        }

        Debug.Log("<color=green> Slow applied </color>");
        Debug.Log("MovementSpeed: " + m_MovementSpeed + ", Initial MovementSpeed: " + m_InitialMovementSpeed);

        Utilities.InvokeDelayed(() => { m_MovementSpeed = m_InitialMovementSpeed; }, debuff.GetDuration(), this);
    }

    private void RemoveSlow(Debuff debuff) {
        m_MovementSpeed = m_InitialMovementSpeed; // TODO: handle possible existing coroutines for the same debuff
    }

    private void ShowDamageUI(float damage) {
        // Instantiate the damage number prefab as a child of the canvas
        EnemyDamageNumberHelper enemyDamageNumberHelper = Instantiate(m_EnemyDamageNumberPrefab, m_Canvas.transform)
            .GetComponent<EnemyDamageNumberHelper>();

        enemyDamageNumberHelper.Initialize(damage.ToString());
    }

    private void UpdateHealthBar() {
        float healthPercentage = m_CurrentHealth / m_MaxHealth;

        m_HealthBar.rectTransform.sizeDelta = new Vector2(
            healthPercentage * m_InitialHealthBarWidth,
            m_HealthBar.rectTransform.sizeDelta.y
        );
    }


    private void OnCollisionStay(Collision other) {
        if (other.gameObject.CompareTag("Player")) {
            if (m_IsDead) return;

            Champion champion = other.gameObject.GetComponent<Champion>();
            if (champion == null) {
                Debug.LogError("Missing champion component");
                return;
            }

            if (Time.time > m_LastAttackTime + m_AttackCooldown) {
                champion.TakeFlatDamage(70f);
                m_LastAttackTime = Time.time;
            }
        }
    }

    public float GetXP() {
        return this.m_RewardXP;
    }

    public float GetMaxHealth() {
        return m_MaxHealth;
    }
}