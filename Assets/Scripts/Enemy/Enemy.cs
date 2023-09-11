using Events;
using System;
using System.Collections.Generic;
using Champions;
using UnityEngine;
using UnityEngine.UI;
using Util;
using Logger = Util.Logger;

public class Enemy : MonoBehaviour, IStackableLivingEntity, IDebuffable {
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
    [SerializeField] private Image m_HealthBarBackground;
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

    public List<Debuff> Debuffs { get; } = new();

    public bool IsBurning { get; }

    public bool IsFragile => Stacks.FindAll(stack => stack.GetStackType() == Stack.StackType.FRAGILE).Count > 0;

    public List<Stack> Stacks { get; } = new();


    private void OnMouseEnter() {
        EventBus<EnemyStartHoverEvent>.Raise(new EnemyStartHoverEvent());
    }

    private void OnMouseExit() {
        EventBus<EnemyStopHoverEvent>.Raise(new EnemyStopHoverEvent());
    }

    private void Start() {
        m_Rigidbody = GetComponent<Rigidbody>();

        m_HealthBar.enabled = false;
        m_HealthBarBackground.enabled = false;

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
        } else if (m_Rigidbody.velocity.x < 0) {
            m_CurrentDir = (m_Rigidbody.velocity.z < 0) ? 2 : 3;
        }


        //If dir changed, flip sprite
        if (m_CurrentDir != m_NewDir) {
            m_NewDir = m_CurrentDir;
            m_Animator.SetTrigger("DirChange");
            if (m_CurrentDir == 0) {
                m_SpriteRenderer.flipX = true;
                m_Animator.SetInteger("Dir", 1);
            } else if (m_CurrentDir == 1) {
                m_SpriteRenderer.flipX = true;
                m_Animator.SetInteger("Dir", 0);
            } else if (m_CurrentDir == 2) {
                m_SpriteRenderer.flipX = false;
                m_Animator.SetInteger("Dir", 0);
            } else if (m_CurrentDir == 3) {
                m_SpriteRenderer.flipX = false;
                m_Animator.SetInteger("Dir", 1);
            }
        }

        Xspeed = m_Rigidbody.velocity.x;
        Zspeed = m_Rigidbody.velocity.z;

        CheckStacksForExpiration();
        CheckDebuffsForExpiration();
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
        // Debug.Log("Taking flat damage");
        TakeDamage(damage);
    }


    private void TakeDamage(float damage) {
        if (m_IsDead) return;
        // Debug.Log("Taking damage");
        float damageTaken = CalculateDamage(damage);
        m_CurrentHealth -= damageTaken;

        m_HealthBar.enabled = true;
        m_HealthBarBackground.enabled = true;


        ShowDamageUI(damageTaken);
        UpdateHealthBar();

        if (m_CurrentHealth <= 0) {
            m_CanMove = false;
            m_Rigidbody.velocity = Vector3.zero;

            m_Collider.isTrigger = true;
            m_Rigidbody.useGravity = false;

            Invoke(nameof(Die), 0.5f);
        }
    }

    private float CalculateDamage(float incomingDamage) {
        int amountOfFragileStacks = Stacks.FindAll(x => x.GetStackType() == Stack.StackType.FRAGILE).Count;
        if (IsFragile) {
            incomingDamage *= 1 + amountOfFragileStacks / 100f;
        }

        return incomingDamage;
    }


    public void AddStacks(int stacks, Stack.StackType stackType) {
        switch (stackType) {
            case Stack.StackType.FRAGILE:
                AddFragileStacks(stacks);
                break;
            case Stack.StackType.DEFTNESS:
                throw new NotImplementedException();
            case Stack.StackType.OVERPOWER:
                throw new NotImplementedException();
        }
    }

    public void RemoveStacks(int stacks, Stack.StackType stackType) {
        switch (stackType) {
            case Stack.StackType.FRAGILE:
                RemoveFragileStacks(stacks);
                break;
            case Stack.StackType.DEFTNESS:
                throw new NotImplementedException();
            case Stack.StackType.OVERPOWER:
                throw new NotImplementedException();
        }
    }

    public void RemoveStack(Stack stack) {
        Stacks.Remove(stack);
    }

    public void CheckStacksForExpiration() {
        // Stacks.ForEach(stack => { stack.CheckForExpiration(); });
        for (int i = Stacks.Count - 1; i >= 0; i--) {
            Stacks[i].CheckForExpiration();
        }
    }

    private void AddFragileStacks(int count) {
        for (int i = 0; i < count; i++) {
            Stack stack = new Stack(Stack.StackType.FRAGILE, this);
            Stacks.Add(stack);
            Logger.Log("Added fragile stack: " + i, Logger.Color.YELLOW, this);
        }
    }

    private void RemoveFragileStacks(int count) {
        // Remove the last count stacks
        int removed = 0;
        for (int i = Stacks.Count - 1; i >= 0; i--) {
            if (removed == count) break;
            if (Stacks[i].GetStackType() == Stack.StackType.FRAGILE) {
                Stacks.RemoveAt(i);
                removed++;
            }
        }
    }

    public void Die() {
        EventBus<EnemyKilledEvent>.Raise(new EnemyKilledEvent(m_Collider, this, transform.position));

        Destroy(gameObject);
    }

    public void ApplyDebuff(Debuff debuff) {
        Debuffs.Add(debuff);
        switch (debuff.GetDebuffType()) {
            case Debuff.DebuffType.Slow:
                ApplySlow(debuff);
                break;
        }
    }

    public void RemoveDebuff(Debuff debuff) {
        Debuffs.Remove(debuff);
        // Debug.Log("Removed debuff: " + debuff.GetDebuffType());
        switch (debuff.GetDebuffType()) {
            case Debuff.DebuffType.Slow:
                RemoveSlow(debuff);
                break;
        }
        // throw new NotImplementedException();
    }

    public void CheckDebuffsForExpiration() {
        // Debuffs.ForEach(debuff => { debuff.CheckForExpiration(); });
        for (int i = Debuffs.Count - 1; i >= 0; i--) {
            Debuffs[i].CheckForExpiration();
        }
    }

    private void ApplySlow(Debuff debuff) {
        // m_MovementSpeed *= value;
        // The value is 0.33, how can i decrease the speed by 33%?
        m_MovementSpeed *= 1 - debuff.GetValue();

        if (debuff.GetDuration() < 0) {
            return;
        }

        Logger.Log("Slow applied", Logger.Color.GREEN, this);
        Logger.Log("MovementSpeed: " + m_MovementSpeed + ", Initial MovementSpeed: " + m_InitialMovementSpeed,
            Logger.Color.BLUE, this);

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
                Logger.Log("Missing champion component", Logger.Color.RED, this);
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

    public Collider GetCollider() {
        return m_Collider;
    }

    public void SetCanMove(bool canMove) {
        m_CanMove = canMove;
    }
}