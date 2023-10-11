using System;
using System.Collections.Generic;
using BuffsDebuffs;
using BuffsDebuffs.Stacks;
using Champions;
using Entities;
using EventBus;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.UI;
using Util;
using Logger = Util.Logger;
using Random = UnityEngine.Random;

namespace Enemy {
    public class Enemy : MonoBehaviour, IStackableLivingEntity, IDebuffable, IDamager {
        public IDamageable currentTarget { get; set; }
        [SerializeField] private GameObject m_EnemyDamageNumberPrefab;
        private Canvas canvas;

        private Rigidbody rigidbody;
        [SerializeField] private Collider m_Collider;
        [SerializeField] private float movementSpeed = 2.2f;
        private float initialMovementSpeed;


        [SerializeField] private float maxHealth = 12f;
        [SerializeField] private float currentHealth = 12f;
        private float damage = 1f;
        [SerializeField] private Image m_HealthBar;
        [SerializeField] private Image m_HealthBarBackground;
        private float initialHealthBarWidth;
        private bool canMove = true;
        private bool canAttack = true;
        private bool isDummy = false;

        private float lastAttackTime = 0f;

        [SerializeField] private float attackCooldown = 3f;

        [SerializeField] private float rewardXP = 1f;

        private SpriteRenderer spriteRenderer;
        private Animator animator;
        [SerializeField] public Material material;
        [SerializeField] private int currentDir = 0;
        private int newDir = 0;
        [SerializeField] float Xspeed;
        [SerializeField] float Zspeed;

        public Renderer renderer;
        [SerializeField] public bool focusAnim;
        [SerializeField] private float intensityMultiplier = 1f;

        private bool isDead {
            get => currentHealth <= 0;
        }

        public List<Debuff> Debuffs { get; } = new();

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
            renderer = GetComponent<Renderer>();

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

            initialHealthBarWidth = m_HealthBar.rectTransform.sizeDelta.x;
            UpdateHealthBar();

            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();

            // randomize the movement a little bit to make the crowd of enemies more interesting
            movementSpeed = Random.Range(movementSpeed - 1f, movementSpeed + 1f);

            initialMovementSpeed = movementSpeed;
        }


        public void SetTarget(IDamageable target) {
            currentTarget = target;
        }

        private void Update() {
            if (isDummy) {
                rigidbody.velocity = Vector3.zero;
            }

            if (currentTarget != null && canMove) {
                Move();
            }

            //Check dir
            if (rigidbody.velocity.x > 0) {
                currentDir = (rigidbody.velocity.z > 0) ? 0 : 1;
            } else if (rigidbody.velocity.x < 0) {
                currentDir = (rigidbody.velocity.z < 0) ? 2 : 3;
            }


            //If dir changed, flip sprite
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

            Xspeed = rigidbody.velocity.x;
            Zspeed = rigidbody.velocity.z;

            CheckStacksForExpiration();
            CheckDebuffsForExpiration();

            if (focusAnim) {
                if (renderer.material.GetVector("_Color").magnitude <
                    2.2f * new Vector4(Color.red.r, Color.red.g, Color.red.b, Color.red.a).magnitude) {
                    // Debug.Log("Increasing intensity...");
                    renderer.material.SetVector("_Color", Color.red * intensityMultiplier);
                    intensityMultiplier += 0.03f;
                } else {
                    focusAnim = false;
                }
            }

            if (!focusAnim) {
                if (renderer.material.GetVector("_Color").x > (Color.red).r) {
                    // Debug.Log("Decreasing intensity...");
                    renderer.material.SetVector("_Color", Color.red * intensityMultiplier);
                    intensityMultiplier -= 0.03f;
                }
            }
        }

        private void Move() {
            if (isDummy) return;
            Vector3 direction = currentTarget.GetTransform().position - transform.position;

            if (direction.magnitude < 0.1f) {
                currentTarget = null;
                return;
            }

            rigidbody.velocity = direction.normalized * movementSpeed;
        }


        public void TakeFlatDamage(float damage) {
            // Debug.Log("Taking flat damage");
            TakeDamage(damage);
        }


        private void TakeDamage(float damage) {
            if (isDead) return;
            // Debug.Log("Taking damage");
            float damageTaken = CalculateDamage(damage);
            currentHealth -= damageTaken;

            m_HealthBar.enabled = true;
            m_HealthBarBackground.enabled = true;


            ShowDamageUI(damageTaken);
            UpdateHealthBar();

            if (currentHealth <= 0) {
                canMove = false;
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

        public void ResetCurrentTarget() {
            throw new NotImplementedException();
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
            movementSpeed *= 1 - debuff.GetValue();

            if (debuff.GetDuration() < 0) {
                return;
            }

            // Logger.Log("Slow applied", Logger.Color.GREEN, this);
            // Logger.Log("MovementSpeed: " + movementSpeed + ", Initial MovementSpeed: " + initialMovementSpeed,
            //     Logger.Color.BLUE, this);

            Utilities.InvokeDelayed(() => { movementSpeed = initialMovementSpeed; }, debuff.GetDuration(), this);
        }

        private void RemoveSlow(Debuff debuff) {
            movementSpeed = initialMovementSpeed; // TODO: handle possible existing coroutines for the same debuff
        }

        private void ShowDamageUI(float damage) {
            // Instantiate the damage number prefab as a child of the canvas
            EnemyDamageNumberHelper enemyDamageNumberHelper = Instantiate(m_EnemyDamageNumberPrefab, canvas.transform)
                .GetComponent<EnemyDamageNumberHelper>();

            enemyDamageNumberHelper.Initialize(damage.ToString());
        }

        public void MakeDummy() {
            isDummy = true;
            maxHealth = 999;
            currentHealth = maxHealth;
            damage = 0;
            gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
        }

        private void UpdateHealthBar() {
            float healthPercentage = currentHealth / maxHealth;

            m_HealthBar.rectTransform.sizeDelta = new Vector2(
                healthPercentage * initialHealthBarWidth,
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
                Collision(other.collider);
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Player")) {
                // Logger.Log("Collision with player", this);
                Collision(other);
            }
        }

        private void Collision(Collider other) {
            if (isDead || !canAttack) return;

            Champion champion = other.gameObject.GetComponent<Champion>();
            if (champion == null) {
                Logger.Log("Missing champion component", Logger.Color.RED, this);
                return;
            }

            if (Time.time > lastAttackTime + attackCooldown) {
                // champion.TakeFlatDamage(damage);
                // Logger.Log("Dealing damage", this);
                DealDamage(champion, damage);
                lastAttackTime = Time.time;
            }
        }

        public float GetXP() {
            return this.rewardXP;
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
            if (!canMove) {
                if (rigidbody == null) {
                    this.canMove = canMove;
                    return;
                }

                rigidbody.velocity = Vector3.zero;
            }

            this.canMove = canMove;
        }

        public void SetCanAttack(bool value) {
            canAttack = value;
        }
    }
}