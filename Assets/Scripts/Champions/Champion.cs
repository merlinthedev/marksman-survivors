using System;
using System.Collections.Generic;
using Events;
using UnityEngine;
using Util;
using Logger = UnityEngine.Logger;
using Random = UnityEngine.Random;

namespace Champions {
    public abstract class Champion : AAbilityHolder, IDebuffable, IEntity, IStackableLivingEntity {
        #region Properties

        [Header("References")]
        [SerializeField] protected Rigidbody m_Rigidbody;

        [Header("Stats")]
        [SerializeField] protected ChampionStatistics m_ChampionStatistics;

        private float m_LastManaRegenerationTime = 0f;
        private float m_LastHealthRegenerationTime = 0f;
        private ChampionLevelManager m_ChampionLevelManager;

        [Header("Movement")]
        [SerializeField] protected Vector3 m_MouseHitPoint;

        private Vector3 m_LastKnownDirection = Vector3.zero;
        [SerializeField] protected bool m_Grounded = false;
        [SerializeField] private float m_GroundedRange = 1.4f;
        private float m_GlobalMovementDirectionAngle = 0f;
        private float m_PreviousAngle = 0f;
        public bool IsMoving => m_Rigidbody.velocity.magnitude > 0.001f;

        [Header("Attack")]
        protected Enemy m_CurrentTarget = null;


        private bool m_NextAttackWillCrit = false;
        protected float m_LastAttackTime = 0f;
        private float m_DamageMultiplier = 1f;

        protected bool m_IsAutoAttacking = false;

        public bool CanAttack {
            get => !m_IsAutoAttacking && Time.time > m_LastAttackTime + (1f / GetAttackSpeed());
        }

        //Buff/Debuff
        public List<Debuff> Debuffs { get; } = new();
        public List<Stack> Stacks { get; } = new();
        public bool IsBurning { get; }
        public bool IsFragile { get; }

        #endregion

        #region OnEnable/OnDisable

        private void OnEnable() {
            EventBus<EnemyKilledEvent>.Subscribe(OnEnemyKilledEvent);
        }

        private void OnDisable() {
            EventBus<EnemyKilledEvent>.Unsubscribe(OnEnemyKilledEvent);
        }

        #endregion

        #region Start and Update

        protected virtual void Start() {
            m_ChampionLevelManager = new ChampionLevelManager(this);
            m_ChampionStatistics.Initialize();

            //Update Health
            EventBus<UpdateResourceBarEvent>.Raise(new UpdateResourceBarEvent("Health",
                m_ChampionStatistics.CurrentHealth,
                m_ChampionStatistics.MaxHealth));
        }

        protected virtual void Update() {
            GroundCheck();

            if (m_MouseHitPoint != Vector3.zero) {
                if (m_Grounded) {
                    OnMove();
                }
            }

            if (!IsMoving && m_CurrentTarget != null) {
                OnAutoAttack(m_CurrentTarget.GetCollider());
            }

            RegenerateResources();
            CheckStacksForExpiration();
            CheckDebuffsForExpiration();

            m_Grounded = false;
        }

        #endregion

        #region Abstract Methods

        public abstract void OnAutoAttack(Collider collider);

        public abstract void OnAbility(KeyCode keyCode);

        #endregion

        #region Debuffs

        public void RemoveDebuff(Debuff debuff) {
            Debuffs.Remove(debuff);
        }

        public void ApplyDebuff(Debuff debuff) {
            Debuffs.Add(debuff);

            switch (debuff.GetDebuffType()) {
                case Debuff.DebuffType.Slow:
                    ApplySlow(debuff);
                    break;
                case Debuff.DebuffType.Burn:
                    ApplyBurn(debuff);
                    break;
            }
        }

        private void ApplySlow(Debuff debuff) {
            m_ChampionStatistics.MovementSpeed *= 1 - debuff.GetValue();

            if (debuff.GetDuration() < 0) {
                return;
            }

            Utilities.InvokeDelayed(
                () => {
                    m_ChampionStatistics.MovementSpeed = m_ChampionStatistics.InitialMovementSpeed;
                    Debuffs.Remove(debuff);
                },
                debuff.GetDuration(), this);
        }

        private void ApplyBurn(Debuff debuff) {
            Debuffs.Add(debuff);
        }

        public void CheckDebuffsForExpiration() {
            // AffectedEntities.ForEach(entity => { entity.Debuffs.ForEach(debuff => { debuff.CheckForExpiration(); }); });
            // Debuffs.ForEach(debuff => { debuff.CheckForExpiration(); });
            for (int i = Debuffs.Count - 1; i >= 0; i--) {
                Debuffs[i].CheckForExpiration();
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
                    AddDeftnessStacks(stacks);
                    break;
                case Stack.StackType.OVERPOWER:
                    AddOverpowerStacks(stacks);
                    break;
            }
        }

        public void RemoveStacks(int stacks, Stack.StackType stackType) {
            switch (stackType) {
                case Stack.StackType.FRAGILE:
                    RemoveFragileStacks(stacks);
                    break;
                case Stack.StackType.DEFTNESS:
                    RemoveDeftnessStacks(stacks);
                    break;
                case Stack.StackType.OVERPOWER:
                    RemoveOverpowerStacks(stacks);
                    break;
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

        private void AddDeftnessStacks(int count) {
            for (int i = 0; i < count; i++) {
                if (Stacks.FindAll(stack => stack.GetStackType() == Stack.StackType.DEFTNESS).Count >= 100) {
                    // get the difference between the inital count and the current count
                    int difference = count - i;
                    AddOverpowerStacks(difference);
                    break;
                }

                Stack stack = new Stack(Stack.StackType.DEFTNESS, this);
                Stacks.Add(stack);
            }
        }

        private void RemoveDeftnessStacks(int count) {
            int removed = 0;
            for (int i = Stacks.Count - 1; i >= 0; i--) {
                if (removed == count) break;
                if (Stacks[i].GetStackType() == Stack.StackType.DEFTNESS) {
                    Stacks.RemoveAt(i);
                    removed++;
                }
            }
        }

        private void AddOverpowerStacks(int count) {
            for (int i = 0; i < count; i++) {
                Stack stack = new Stack(Stack.StackType.OVERPOWER, this);
                Stacks.Add(stack);
            }
        }

        private void RemoveOverpowerStacks(int count) {
            int removed = 0;
            for (int i = Stacks.Count - 1; i >= 0; i--) {
                if (removed == count) break;
                if (Stacks[i].GetStackType() == Stack.StackType.OVERPOWER) {
                    Stacks.RemoveAt(i);
                    removed++;
                }
            }
        }

        #endregion

        #region Resources

        private void RegenerateResources() {
            TryRegenerateHealth();
            TryRegenerateMana();
        }

        private void TryRegenerateHealth() {
            if (m_ChampionStatistics.CurrentHealth < m_ChampionStatistics.MaxHealth) {
                if (Time.time > m_LastHealthRegenerationTime + 1f) {
                    m_ChampionStatistics.CurrentHealth += m_ChampionStatistics.HealthRegen;
                    m_LastHealthRegenerationTime = Time.time;
                    EventBus<ChampionHealthRegenerated>.Raise(new ChampionHealthRegenerated());
                }
            }
        }


        private void TryRegenerateMana() {
            if (m_ChampionStatistics.CurrentMana < m_ChampionStatistics.MaxMana) {
                if (Time.time > m_LastManaRegenerationTime + 1f) {
                    m_ChampionStatistics.CurrentMana += m_ChampionStatistics.ManaRegen;
                    m_LastManaRegenerationTime = Time.time;
                    EventBus<ChampionManaRegenerated>.Raise(new ChampionManaRegenerated());
                }
            }
        }

        #endregion

        #region Movement

        protected virtual void OnMove() {
            if (m_MouseHitPoint == Vector3.zero) {
                return;
            }

            // Debug.Log("Moving");
            Vector3 direction = m_MouseHitPoint - transform.position;

            m_PreviousAngle = m_GlobalMovementDirectionAngle;
            m_GlobalMovementDirectionAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            if (direction.magnitude < 0.1f) {
                // Debug.Log("Stop moving");
                m_GlobalMovementDirectionAngle = m_PreviousAngle;
                m_MouseHitPoint = Vector3.zero;
                m_Rigidbody.velocity = Vector3.zero;
                return;
            }


            int deftnessStacks = Stacks.FindAll(stack => stack.GetStackType() == Stack.StackType.DEFTNESS).Count;

            if (deftnessStacks > 0) {
                Util.Logger.Log("Deftness stacks: " + deftnessStacks, Util.Logger.Color.GREEN, this);
            }


            m_Rigidbody.velocity = direction.normalized *
                                   (m_ChampionStatistics.MovementSpeed * (1f + deftnessStacks / 100f));

            Util.Logger.Log("Velocity: " + m_Rigidbody.velocity.magnitude, Util.Logger.Color.GREEN, this);
            m_LastKnownDirection = direction.normalized;
        }

        public void Stop() {
            m_Rigidbody.velocity = Vector3.zero;
            m_MouseHitPoint = Vector3.zero;
        }

        private void GroundCheck() {
            // Debug.Log("Ground check");
            // Debug.DrawRay(transform.position, Vector3.down * 10f, Color.red, 0);
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, m_GroundedRange, 1 << 6)) {
                // Debug.Log("Ground check hit");
                if (hit.collider.CompareTag("Ground")) {
                    // Debug.Log("Ground check hit ground");
                    m_Grounded = true;
                }
            }
        }

        #endregion

        #region Damage & Death

        protected virtual void OnDamageTaken(float damage) {
            float fragileStacks = Stacks.FindAll(stack => stack.GetStackType() == Stack.StackType.FRAGILE).Count;
            damage = IsFragile ? damage * 1 + fragileStacks / 10 : damage;
            m_ChampionStatistics.CurrentHealth -= damage;
            EventBus<ChampionDamageTakenEvent>.Raise(new ChampionDamageTakenEvent());
            if (m_ChampionStatistics.CurrentHealth <= 0) {
                OnDeath();
            }
        }

        public void TakeFlatDamage(float damage) {
            OnDamageTaken(damage);
        }

        protected float CalculateDamage() {
            float damage = GetAttackDamage();

            if (Random.value < m_ChampionStatistics.CriticalStrikeChance || m_NextAttackWillCrit) {
                damage *= (1 + m_ChampionStatistics.CriticalStrikeDamage);
            }

            if (m_NextAttackWillCrit) m_NextAttackWillCrit = false;

            return damage;
        }

        protected virtual void OnDeath() {
            // Death logic

            Destroy(gameObject);
        }

        #endregion

        #region Events

        private void OnEnemyKilledEvent(EnemyKilledEvent e) {
            // TODO: XP
            m_ChampionStatistics.CurrentXP += e.m_Enemy.GetXP();
            m_ChampionLevelManager.CheckForLevelUp();
            EventBus<UpdateResourceBarEvent>.Raise(new UpdateResourceBarEvent("XP", m_ChampionStatistics.CurrentXP,
                m_ChampionLevelManager.CurrentLevelXP));
        }

        #endregion

        #region Debug

        protected void DrawDirectionRays() {
            // Direction ray
            Debug.DrawRay(transform.position, GetCurrentMovementDirection() * 10f, Color.red);

            // Right ray
            Vector3 rightDirection = Quaternion.Euler(0, 45, 0) * GetCurrentMovementDirection();
            // Debug.DrawRay(transform.position, rightDirection * 10f, Color.green);
            // Draw the ray until the edge of the viewport
            Debug.DrawRay(transform.position, rightDirection * 10f, Color.green, 0);

            // Left ray
            Vector3 leftDirection = Quaternion.Euler(0, -45, 0) * GetCurrentMovementDirection();
            Debug.DrawRay(transform.position, leftDirection * 10f, Color.blue, 0);
        }

        #endregion

        #region Getters & Setters

        public float GetCurrentHealth() => m_ChampionStatistics.CurrentHealth;
        public float GetMaxHealth() => m_ChampionStatistics.MaxHealth;

        public float GetAttackSpeed() => m_ChampionStatistics.GetAttackSpeed(1 +
                                                                             (Stacks.FindAll(stack =>
                                                                                  stack.GetStackType() ==
                                                                                  Stack.StackType.DEFTNESS).Count /
                                                                              100f));

        public float GetAttackDamage() => m_ChampionStatistics.GetAttackDamage(1 +
                                                                               (Stacks.FindAll(stack =>
                                                                                           stack.GetStackType() ==
                                                                                           Stack.StackType.OVERPOWER)
                                                                                       .Count /
                                                                                   100f));

        public float GetLastAttackTime() => m_LastAttackTime;
        protected float GetDamageMultiplier() => m_DamageMultiplier;
        public Rigidbody GetRigidbody() => m_Rigidbody;
        public ChampionStatistics GetChampionStatistics() => m_ChampionStatistics;
        public ChampionLevelManager GetChampionLevelManager() => m_ChampionLevelManager;

        public Vector3 GetCurrentMovementDirection() {
            return m_Rigidbody.velocity.normalized == Vector3.zero
                ? m_LastKnownDirection
                : m_Rigidbody.velocity.normalized;
        }

        public float GetGlobalDirectionAngle() {
            // return the angle but instead of -180-180 i want it to be 0-360
            return m_GlobalMovementDirectionAngle < 0
                ? m_GlobalMovementDirectionAngle + 360
                : m_GlobalMovementDirectionAngle;
        }

        public void SetMouseHitPoint(Vector3 point) {
            // set the target enemy to null to stop us from auto attacking it again when we reach our destination
            m_CurrentTarget = null;
            m_MouseHitPoint = point;
        }

        protected void SetGlobalDirectionAngle(float angle) {
            m_GlobalMovementDirectionAngle = angle;
        }

        public void SetNextAttackWillCrit(bool b) {
            m_NextAttackWillCrit = b;
        }

        #endregion
    }
}