using System;
using System.Collections.Generic;
using Events;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace Champions {
    public abstract class Champion : AAbilityHolder, IDebuffable, IEntity,
        IStackableLivingEntity {
        [SerializeField] protected Rigidbody m_Rigidbody;

        [SerializeField] protected Vector3 m_MouseHitPoint;
        private Vector3 m_LastKnownDirection = Vector3.zero;

        [SerializeField] protected ChampionStatistics m_ChampionStatistics;

        private ChampionLevelManager m_ChampionLevelManager;

        [SerializeField] private float m_GroundedRange = 1.4f;
        protected float m_LastAttackTime = 0f;
        private float m_GlobalMovementDirectionAngle = 0f;
        private float m_MovementMultiplier = 1f;
        private float m_DamageMultiplier = 1f;
        private float m_PreviousAngle = 0f;


        [SerializeField] protected bool m_CanMove = true;
        [SerializeField] protected bool m_Grounded = false;
        protected bool m_HasAttackCooldown = false;
        private bool m_NextAttackWillCrit = false;

        public bool IsBurning { get; }
        public bool IsFragile { get; }

        public List<Debuff> Debuffs { get; } = new();
        public List<Stack> Stacks { get; } = new();

        public bool CanAttack {
            get => !m_HasAttackCooldown;
            protected set => m_HasAttackCooldown = !value;
        }

        public bool IsMoving => m_Rigidbody.velocity.magnitude > 0.001f;

        private void OnEnable() {
            // Debug.Log("Champion onEnable");

            EventBus<EnemyKilledEvent>.Subscribe(OnEnemyKilledEvent);
        }

        private void OnDisable() {
            // Debug.Log("Champion onDisable");

            EventBus<EnemyKilledEvent>.Unsubscribe(OnEnemyKilledEvent);
        }

        public abstract void OnAutoAttack(Collider collider);

        public abstract void OnAbility(KeyCode keyCode);

        public void TakeFlatDamage(float damage) {
            OnDamageTaken(damage);
        }


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

        public void CheckDebuffsForExpiration() {
            // AffectedEntities.ForEach(entity => { entity.Debuffs.ForEach(debuff => { debuff.CheckForExpiration(); }); });
            // Debuffs.ForEach(debuff => { debuff.CheckForExpiration(); });
            for (int i = Debuffs.Count - 1; i >= 0; i--) {
                Debuffs[i].CheckForExpiration();
            }
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


        protected virtual void Start() {
            m_ChampionLevelManager = new ChampionLevelManager(this);

            m_ChampionStatistics.Initialize();

            //Update Health
            EventBus<UpdateResourceBarEvent>.Raise(new UpdateResourceBarEvent("Health", m_ChampionStatistics.CurrentHealth,
                m_ChampionStatistics.MaxHealth));
        }

        protected virtual void Update() {
            GroundCheck();

            if (m_MouseHitPoint != Vector3.zero) {
                if (m_CanMove && m_Grounded) {
                    OnMove();
                }
            }
            // Debug.Log("IsGrounded: " + m_Grounded);


            RegenerateResources();
            CheckStacksForExpiration();
            CheckDebuffsForExpiration();
            m_Grounded = false;
        }

        private void RegenerateResources() {
            TryRegenerateHealth();
            TryRegenerateMana();
        }

        private float m_LastHealthRegenerationTime = 0f;

        private void TryRegenerateHealth() {
            if (m_ChampionStatistics.CurrentHealth < m_ChampionStatistics.MaxHealth) {
                if (Time.time > m_LastHealthRegenerationTime + 1f) {
                    m_ChampionStatistics.CurrentHealth += m_ChampionStatistics.HealthRegen;
                    m_LastHealthRegenerationTime = Time.time;
                    EventBus<ChampionHealthRegenerated>.Raise(new ChampionHealthRegenerated());
                }
            }
        }

        private float m_LastManaRegenerationTime = 0f;

        private void TryRegenerateMana() {
            if (m_ChampionStatistics.CurrentMana < m_ChampionStatistics.MaxMana) {
                if (Time.time > m_LastManaRegenerationTime + 1f) {
                    m_ChampionStatistics.CurrentMana += m_ChampionStatistics.ManaRegen;
                    m_LastManaRegenerationTime = Time.time;
                    EventBus<ChampionManaRegenerated>.Raise(new ChampionManaRegenerated());
                }
            }
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

        protected virtual void OnMove() {
            if (m_MouseHitPoint == Vector3.zero || !m_CanMove) {
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

            m_Rigidbody.velocity = direction.normalized * (m_ChampionStatistics.MovementSpeed * m_MovementMultiplier);
            m_LastKnownDirection = direction.normalized;
        }

        protected virtual void OnDamageTaken(float damage) {
            float fragileStacks = Stacks.FindAll(stack => stack.GetStackType() == Stack.StackType.FRAGILE).Count;
            damage = IsFragile ? damage * 1 + fragileStacks / 10 : damage;
            m_ChampionStatistics.CurrentHealth -= damage;
            EventBus<ChampionDamageTakenEvent>.Raise(new ChampionDamageTakenEvent());
            if (m_ChampionStatistics.CurrentHealth <= 0) {
                OnDeath();
            }
        }

        protected virtual void OnDeath() {
            // Death logic

            Destroy(gameObject);
        }


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

        protected float CalculateDamage() {
            float damage = m_ChampionStatistics.AttackDamage;

            if (Random.value < m_ChampionStatistics.CriticalStrikeChance || m_NextAttackWillCrit) {
                damage *= (1 + m_ChampionStatistics.CriticalStrikeDamage);
            }

            if (m_NextAttackWillCrit) m_NextAttackWillCrit = false;

            return damage;
        }

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
            m_MouseHitPoint = point;
        }

        protected void SetGlobalDirectionAngle(float angle) {
            m_GlobalMovementDirectionAngle = angle;
        }

        protected void SetCanMove(bool value) {
            m_CanMove = value;
            // Debug.Log("Move set ton true");
        }

        public void CleanseAllDebuffs() {
            throw new NotImplementedException();
        }

        protected void SetDamageMultiplier(float v) {
            m_DamageMultiplier = v; // hard set the multiplier
        }

        protected void ResetDamageMultiplier() {
            m_DamageMultiplier = 1f;
        }

        public void SetNextAttackWillCrit(bool b) {
            m_NextAttackWillCrit = b;
        }

        private void OnEnemyKilledEvent(EnemyKilledEvent e) {
            // TODO: XP
            m_ChampionStatistics.CurrentXP += e.m_Enemy.GetXP();
            m_ChampionLevelManager.CheckForLevelUp();
            EventBus<UpdateResourceBarEvent>.Raise(new UpdateResourceBarEvent("XP", m_ChampionStatistics.CurrentXP,
                m_ChampionLevelManager.CurrentLevelXP));

        }

        public float GetCurrentHealth() => m_ChampionStatistics.CurrentHealth;
        public float GetMaxHealth() => m_ChampionStatistics.MaxHealth;
        public float GetAttackSpeed() => m_ChampionStatistics.AttackSpeed;
        public float GetLastAttackTime() => m_LastAttackTime;
        protected float GetCurrentMovementMultiplier() => m_MovementMultiplier;
        protected float GetDamageMultiplier() => m_DamageMultiplier;
        public Rigidbody GetRigidbody() => m_Rigidbody;
        public ChampionStatistics GetChampionStatistics() => m_ChampionStatistics;
        public ChampionLevelManager GetChampionLevelManager() => m_ChampionLevelManager;
    }
}