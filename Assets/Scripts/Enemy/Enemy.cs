using System;
using System.Collections.Generic;
using BuffsDebuffs;
using BuffsDebuffs.Stacks;
using Champions;
using Entities;
using EventBus;
using UnityEngine;
using UnityEngine.UI;
using Util;
using Logger = Util.Logger;

namespace Enemy {
    public class Enemy : MonoBehaviour, IStackableLivingEntity, IDebuffable, IDamager {
        private Transform target;
        [SerializeField] private GameObject m_EnemyDamageNumberPrefab;
        private Canvas canvas;

        private Rigidbody rigidbody;
        [SerializeField] private Collider m_Collider;
        [SerializeField] private float m_MovementSpeed = 7f;
        private float initialMovementSpeed;


        [SerializeField] private float maxHealth = 12f;
        [SerializeField] private float currentHealth = 12f;
        private float damage = 1f;
        [SerializeField] private Image m_HealthBar;
        [SerializeField] private Image m_HealthBarBackground;
        private float m_InitialHealthBarWidth;
        private bool m_CanMove = true;
        private bool m_CanAttack = true;

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
            rigidbody = GetComponent<Rigidbody>();

            m_HealthBar.enabled = false;
            m_HealthBarBackground.enabled = false;

            if (rigidbody == null) {
                Debug.LogError("Missing rigidbody");
                throw new Exception("Missing rigidbody");
            }

            canvas = GetComponentInChildren<Canvas>();
            if (canvas == null) {
                Debug.LogError("Missing canvas");
                throw new Exception("Missing canvas");
            }

            m_Collider = GetComponent<Collider>();
            if (m_Collider == null) {
                Debug.LogError("Missing collider");
                throw new Exception("Missing collider");
            }

            m_InitialHealthBarWidth = m_HealthBar.rectTransform.sizeDelta.x;
            UpdateHealthBar();

            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_Animator = GetComponent<Animator>();

            initialMovementSpeed = m_MovementSpeed;
        }


        public void SetTarget(Transform target) {
            this.target = target;
        }

        private void Update() {
            if (target != null && m_CanMove) {
                Move();
            }

            //Check dir
            if (rigidbody.velocity.x > 0) {
                m_CurrentDir = (rigidbody.velocity.z > 0) ? 0 : 1;
            }
            else if (rigidbody.velocity.x < 0) {
                m_CurrentDir = (rigidbody.velocity.z < 0) ? 2 : 3;
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

            Xspeed = rigidbody.velocity.x;
            Zspeed = rigidbody.velocity.z;

            CheckStacksForExpiration();
            CheckDebuffsForExpiration();
        }

        private void Move() {
            Vector3 direction = target.position - transform.position;

            if (direction.magnitude < 0.1f) {
                target = null;
                return;
            }

            rigidbody.velocity = direction.normalized * m_MovementSpeed;
        }


        public void TakeFlatDamage(float damage) {
            // Debug.Log("Taking flat damage");
            TakeDamage(damage);
        }


        private void TakeDamage(float damage) {
            if (m_IsDead) return;
            // Debug.Log("Taking damage");
            float damageTaken = CalculateDamage(damage);
            currentHealth -= damageTaken;

            m_HealthBar.enabled = true;
            m_HealthBarBackground.enabled = true;


            ShowDamageUI(damageTaken);
            UpdateHealthBar();

            if (currentHealth <= 0) {
                m_CanMove = false;
                rigidbody.velocity = Vector3.zero;

                m_Collider.isTrigger = true;
                rigidbody.useGravity = false;

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

        public void DealDamage(IDamageable damageable, float damage) {
            damageable.TakeFlatDamage(damage);
        }

        public void Die() {
            EventBus<EnemyKilledEvent>.Raise(new EnemyKilledEvent(m_Collider, this, transform.position));

            Destroy(gameObject);
        }

        public void ApplyDebuff(Debuff debuff) {
            Debuffs.Add(debuff);
            switch (debuff.GetDebuffType()) {
                case Debuff.DebuffType.SLOW:
                    ApplySlow(debuff);
                    break;
            }
        }

        public void RemoveDebuff(Debuff debuff) {
            Debuffs.Remove(debuff);
            // Debug.Log("Removed debuff: " + debuff.GetDebuffType());
            switch (debuff.GetDebuffType()) {
                case Debuff.DebuffType.SLOW:
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
            Logger.Log("MovementSpeed: " + m_MovementSpeed + ", Initial MovementSpeed: " + initialMovementSpeed,
                Logger.Color.BLUE, this);

            Utilities.InvokeDelayed(() => { m_MovementSpeed = initialMovementSpeed; }, debuff.GetDuration(), this);
        }

        private void RemoveSlow(Debuff debuff) {
            m_MovementSpeed = initialMovementSpeed; // TODO: handle possible existing coroutines for the same debuff
        }

        private void ShowDamageUI(float damage) {
            // Instantiate the damage number prefab as a child of the canvas
            EnemyDamageNumberHelper enemyDamageNumberHelper = Instantiate(m_EnemyDamageNumberPrefab, canvas.transform)
                .GetComponent<EnemyDamageNumberHelper>();

            enemyDamageNumberHelper.Initialize(damage.ToString());
        }

        private void UpdateHealthBar() {
            float healthPercentage = currentHealth / maxHealth;

            m_HealthBar.rectTransform.sizeDelta = new Vector2(
                healthPercentage * m_InitialHealthBarWidth,
                m_HealthBar.rectTransform.sizeDelta.y
            );
        }

        public void OnPause() {
            SetCanMove(false);
            SetCanAttack(false);
        }

        public void OnResume() {
            // Invoking delayed to give the player some time to move away from any very close enemies
            Utilities.InvokeDelayed(() => {
                SetCanMove(true);
                SetCanAttack(true);
            }, 0.2f, this);
        }

        private void OnCollisionStay(Collision other) {
            if (other.gameObject.CompareTag("Player")) {
                if (m_IsDead || !m_CanAttack) return;

                Champion champion = other.gameObject.GetComponent<Champion>();
                if (champion == null) {
                    Logger.Log("Missing champion component", Logger.Color.RED, this);
                    return;
                }

                if (Time.time > m_LastAttackTime + m_AttackCooldown) {
                    // champion.TakeFlatDamage(damage);
                    DealDamage(champion, damage);
                    m_LastAttackTime = Time.time;
                }
            }
        }

        public float GetXP() {
            return this.m_RewardXP;
        }

        public float GetMaxHealth() {
            return maxHealth;
        }

        public Collider GetCollider() {
            return m_Collider;
        }

        public int GetStackAmount(Stack.StackType stackType) {
            return Stacks.FindAll(stack => stack.GetStackType() == stackType).Count;
        }

        public Transform GetTransform() {
            return gameObject.transform;
        }

        public void SetCanMove(bool canMove) {
            m_CanMove = canMove;
        }

        public void SetCanAttack(bool value) {
            m_CanAttack = value;
        }
    }
}