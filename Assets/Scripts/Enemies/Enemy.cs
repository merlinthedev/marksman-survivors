using System;
using System.Collections.Generic;
using BuffsDebuffs;
using BuffsDebuffs.Stacks;
using Entities;
using EventBus;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Enemies {
    public abstract class Enemy : MonoBehaviour, IStackableLivingEntity, IDebuffable, IDamager {
        [Header("RIGIDBODY & COLLIDER")]
        [SerializeField] protected Rigidbody rigidbody;

        [SerializeField] private Collider collider;
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private Renderer renderer;

        [Header("UI")]
        [SerializeField] private Image healthBar;

        [SerializeField] private Image healthBarBackground;
        [SerializeField] private GameObject damageNumberPrefab;
        [SerializeField] private float intensityMultiplier = 1f;
        private Canvas canvas;
        private float initialHealthBarWidth;
        public bool focusAnim;


        [Header("MOVEMENT")]
        [SerializeField] protected float initialMovementSpeed;

        private int currentDir = 0;
        private int newDir = 0;
        protected float movementSpeed = 0;


        [Header("STATS")]
        [SerializeField] private float maxHealth;

        [SerializeField] private float rewardXP;
        private float currentHealth;
        [SerializeField] protected float damage = 1f;

        protected bool isDead => currentHealth <= 0;
        private bool canMove = true;
        protected bool canAttack = true;

        protected float lastAttackTime;
        protected float attackChargeTime = 0.2f;
        protected float attackCooldown = 0.2f;

        public bool IsFragile => Stacks.FindAll(stack => stack.GetStackType() == Stack.StackType.FRAGILE).Count > 0;
        public List<Stack> Stacks { get; } = new();
        public List<Debuff> Debuffs { get; } = new();
        public IDamageable currentTarget { get; set; } = null;

        private void Start() {
            initialHealthBarWidth = healthBar.rectTransform.sizeDelta.x;

            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            renderer = GetComponent<Renderer>();
            canvas = GetComponentInChildren<Canvas>();

            movementSpeed = initialMovementSpeed;
            currentHealth = maxHealth;
            UpdateHealthBar();
        }

        private void Update() {
            if (currentTarget != null && canMove) {
                Move();
            }

            CheckDirection();
            ApplyDirection();

            CheckStacksForExpiration();
            CheckDebuffsForExpiration();

            FocusAnimation();
        }

        private void OnMouseEnter() {
            EventBus<EnemyStartHoverEvent>.Raise(new EnemyStartHoverEvent(this));
        }

        private void OnMouseExit() {
            EventBus<EnemyStopHoverEvent>.Raise(new EnemyStopHoverEvent());
        }

        #region Movement

        protected abstract void Move();

        private void ApplyDirection() {
            if (currentDir != newDir) {
                newDir = currentDir;
                animator.SetTrigger("DirChange");
                if (currentDir == 0) {
                    spriteRenderer.flipX = true;
                    animator.SetInteger("Dir", 1);
                } else if (currentDir == 1) {
                    spriteRenderer.flipX = true;
                    animator.SetInteger("Dir", 0);
                } else if (currentDir == 2) {
                    spriteRenderer.flipX = false;
                    animator.SetInteger("Dir", 0);
                } else if (currentDir == 3) {
                    spriteRenderer.flipX = false;
                    animator.SetInteger("Dir", 1);
                }
            }
        }

        private void CheckDirection() {
            if (rigidbody.velocity.x > 0) {
                currentDir = (rigidbody.velocity.z > 0) ? 0 : 1;
            } else if (rigidbody.velocity.x < 0) {
                currentDir = rigidbody.velocity.z < 0 ? 2 : 3;
            }
        }

        #endregion

        #region Stacks

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

        private void AddFragileStacks(int count) {
            for (int i = 0; i < count; i++) {
                Stack stack = new Stack(Stack.StackType.FRAGILE, this);
                Stacks.Add(stack);
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

        public void RemoveStack(Stack stack) {
            Stacks.Remove(stack);
        }

        public void CheckStacksForExpiration() {
            for (int i = Stacks.Count - 1; i >= 0; i--) {
                Stacks[i].CheckForExpiration();
            }
        }

        public int GetStackAmount(Stack.StackType stackType) {
            return Stacks.FindAll(stack => stack.GetStackType() == stackType).Count;
        }

        #endregion

        #region Damage

        public virtual void TakeFlatDamage(float damage) {
            TakeDamage(damage);
        }

        private void TakeDamage(float damage) {
            if (isDead) return;

            float damageTaken = CalculateIncomingDamage(damage);

            currentHealth -= damageTaken;

            healthBar.enabled = true;
            healthBarBackground.enabled = true;

            ShowDamageUI(damageTaken);
            UpdateHealthBar();

            if (currentHealth <= 0) {
                canMove = false;
                rigidbody.velocity = Vector3.zero;
                rigidbody.useGravity = false;
                collider.isTrigger = true;

                Invoke(nameof(Die), 0.5f);
            }
        }

        public void Die() {
            EventBus<EnemyKilledEvent>.Raise(new EnemyKilledEvent(collider, this, transform.position));

            Destroy(gameObject);
        }

        public float CalculateIncomingDamage(float damage) {
            int amountOfFragileStacks = Stacks.FindAll(stack => stack.GetStackType() == Stack.StackType.FRAGILE).Count;

            if (IsFragile) {
                damage *= 1 + amountOfFragileStacks / 100f;
            }

            return damage;
        }

        public abstract void DealDamage(IDamageable damageable, float damage);

        public void ResetCurrentTarget() {
            currentTarget = null;
        }

        #endregion

        #region Debuffs

        public void ApplyDebuff(Debuff debuff) {
            Debuffs.Add(debuff);

            switch (debuff.GetDebuffType()) {
                case Debuff.DebuffType.SLOW:
                    ApplySlow(debuff);
                    break;
            }
        }

        private void ApplySlow(Debuff debuff) {
            movementSpeed *= 1 - debuff.GetValue();

            if (debuff.GetDuration() < 0) {
                return;
            }

            Utilities.InvokeDelayed(() => { movementSpeed = initialMovementSpeed; }, debuff.GetDuration(), this);
        }

        private void RemoveSlow(Debuff debuff) {
            movementSpeed = initialMovementSpeed;
        }

        public void RemoveDebuff(Debuff debuff) {
            Debuffs.Remove(debuff);
            switch (debuff.GetDebuffType()) {
                case Debuff.DebuffType.SLOW:
                    RemoveSlow(debuff);
                    break;
            }
        }

        public void CheckDebuffsForExpiration() {
            for (var i = Debuffs.Count - 1; i >= 0; i--) {
                Debuffs[i].CheckForExpiration();
            }
        }

        #endregion

        #region UI

        protected void ShowDamageUI(float damage) {
            EnemyDamageNumberHelper enemyDamageNumberHelper = Instantiate(damageNumberPrefab, canvas.transform)
                .GetComponent<EnemyDamageNumberHelper>();

            enemyDamageNumberHelper.Initialize(damage.ToString());
        }

        private void UpdateHealthBar() {
            float healthPercentage = currentHealth / maxHealth;

            healthBar.rectTransform.sizeDelta = new Vector2(healthPercentage * initialHealthBarWidth,
                healthBar.rectTransform.sizeDelta.y);
        }

        private void FocusAnimation() {
            if (focusAnim) {
                if (renderer.material.GetVector("_Color").magnitude <
                    2.2f * new Vector4(Color.red.r, Color.red.g, Color.red.b, Color.red.a).magnitude) {
                    // Debug.Log("Increasing intensity...");
                    renderer.material.SetVector("_Color", Color.red * intensityMultiplier);
                    intensityMultiplier += 0.03f;
                } else {
                    focusAnim = false;
                }
            } else {
                if (renderer.material.GetVector("_Color").x > (Color.red).r) {
                    // Debug.Log("Decreasing intensity...");
                    renderer.material.SetVector("_Color", Color.red * intensityMultiplier);
                    intensityMultiplier -= 0.03f;
                }
            }
        }

        #endregion

        #region Pause & Resume

        public void OnPause() {
            SetCanAttack(false);
            SetCanMove(false);
        }

        public void OnResume() {
            Utilities.InvokeDelayed(() => {
                SetCanMove(true);
                SetCanAttack(true);
            }, 0.2f, this);
        }

        #endregion

        #region Getters & Setters

        public Transform GetTransform() {
            return gameObject.transform;
        }

        public float GetMaxHealth() {
            return maxHealth;
        }

        public float GetXP() {
            return rewardXP;
        }

        public void SetCanMove(bool canMove) {
            if (!canMove) {
                if (rigidbody == null) {
                    this.canMove = canMove;
                    return;
                }

                rigidbody.velocity = Vector3.zero;
            }

            this.canMove = canMove;
        }

        public void SetCanAttack(bool canAttack) {
            this.canAttack = canAttack;
        }

        public void SetTarget(IDamageable damageable) {
            currentTarget = damageable;
        }

        #endregion

        public enum AttackType {
            NONE,
            BASIC,
            AOE,
            BUFF
        }

        public enum EnemyType {
            BASIC,
            CASTER,
            SPEED_SHAMAN,
        }
    }
}