using System.Collections.Generic;
using System.Diagnostics;
using BuffsDebuffs;
using BuffsDebuffs.Stacks;
using Champions.Abilities;
using EventBus;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Util;
using Debug = UnityEngine.Debug;
using Logger = Util.Logger;
using Random = UnityEngine.Random;

namespace Champions {
    public abstract class Champion : AAbilityHolder, IDebuffable, IStackableLivingEntity {
        #region Properties

        [Header("References")]
        [SerializeField] protected Rigidbody rigidbody;

        [Header("Stats")]
        [SerializeField] protected ChampionStatistics championStatistics;

        private float lastManaRegenerationTime = 0f;
        private float lastHealthRegenerationTime = 0f;
        private ChampionLevelManager championLevelManager;

        [FormerlySerializedAs("m_MouseHitPoint")]
        [Header("Movement")]
        [SerializeField]
        protected Vector3 mouseHitPoint;

        private Vector3 lastKnownDirection = Vector3.zero;

        [FormerlySerializedAs("m_Grounded")] [SerializeField]
        protected bool grounded = false;

        [FormerlySerializedAs("m_GroundedRange")] [SerializeField]
        private float groundedRange = 1.4f;

        private float globalMovementDirectionAngle = 0f;
        private float previousAngle = 0f;
        public bool IsMoving => rigidbody.velocity.magnitude > 0.001f;

        [Header("Attack")]
        protected Enemy.Enemy currentTarget = null;


        private bool nextAttackWillCrit = false;
        protected float lastAttackTime = 0f;
        private float damageMultiplier = 1f;

        protected bool isAutoAttacking = false;

        protected bool CanAttack {
            get => !isAutoAttacking && Time.time > lastAttackTime + (1f / GetAttackSpeed());
        }

        //Buff/Debuff
        public List<Debuff> Debuffs { get; } = new();
        public List<Stack> Stacks { get; } = new();
        public bool IsBurning { get; }
        public bool IsFragile { get; }

        #endregion

        #region OnEnable/OnDisable

        private void OnEnable() {
            // Logger.excludedContexts.Add(this);

            EventBus<EnemyKilledEvent>.Subscribe(OnEnemyKilledEvent);
            EventBus<ChampionAbilityChosenEvent>.Subscribe(OnChampionAbilityChosen);
        }

        private void OnDisable() {
            EventBus<EnemyKilledEvent>.Unsubscribe(OnEnemyKilledEvent);
            EventBus<ChampionAbilityChosenEvent>.Unsubscribe(OnChampionAbilityChosen);
        }

        #endregion

        #region Start and Update

        protected virtual void Start() {
            championLevelManager = new ChampionLevelManager(this);
            championStatistics.Initialize();

            //Update Health
            EventBus<UpdateResourceBarEvent>.Raise(new UpdateResourceBarEvent("Health",
                championStatistics.CurrentHealth,
                championStatistics.MaxHealth));
        }

        protected virtual void FixedUpdate() {
            GroundCheck();

            if (mouseHitPoint != Vector3.zero) {
                if (grounded) {
                    OnMove();
                }
            }

            grounded = false;
        }

        protected virtual void Update() {
            if (!IsMoving && currentTarget != null) {
                OnAutoAttack(currentTarget.GetCollider());
            }

            RegenerateResources();
            CheckStacksForExpiration();
            CheckDebuffsForExpiration();


            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                AddStacks(1, Stack.StackType.FRAGILE);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                AddStacks(1, Stack.StackType.OVERPOWER);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3)) {
                AddStacks(1, Stack.StackType.DEFTNESS);
            }
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
            championStatistics.MovementSpeed *= 1 - debuff.GetValue();

            if (debuff.GetDuration() < 0) {
                return;
            }

            Utilities.InvokeDelayed(
                () => {
                    championStatistics.MovementSpeed = championStatistics.InitialMovementSpeed;
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

            EventBus<ChangeStackUIEvent>.Raise(new ChangeStackUIEvent(stackType, stacks, true));
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
            EventBus<ChangeStackUIEvent>.Raise(new ChangeStackUIEvent(stack.GetStackType(), 1, false));
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
                if (Stacks.FindAll(stack => stack.GetStackType() == Stack.StackType.DEFTNESS).Count >= 25) {
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
            if (championStatistics.CurrentHealth < championStatistics.MaxHealth) {
                if (Time.time > lastHealthRegenerationTime + 1f) {
                    championStatistics.CurrentHealth += championStatistics.HealthRegen;
                    lastHealthRegenerationTime = Time.time;
                    EventBus<ChampionHealthRegenerated>.Raise(new ChampionHealthRegenerated());
                }
            }
        }


        private void TryRegenerateMana() {
            if (championStatistics.CurrentMana < championStatistics.MaxMana) {
                if (Time.time > lastManaRegenerationTime + 1f) {
                    championStatistics.CurrentMana += championStatistics.ManaRegen;
                    lastManaRegenerationTime = Time.time;
                    EventBus<ChampionManaRegenerated>.Raise(new ChampionManaRegenerated());
                }
            }
        }

        #endregion

        #region Movement

        protected virtual void OnMove() {
            if (mouseHitPoint == Vector3.zero) {
                return;
            }

            // Debug.Log("Moving");
            Vector3 direction = mouseHitPoint - transform.position;

            previousAngle = globalMovementDirectionAngle;
            globalMovementDirectionAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            // if (direction.magnitude < 0.1f) {
            //     // Debug.Log("Stop moving");
            //     globalMovementDirectionAngle = previousAngle;
            //     mouseHitPoint = Vector3.zero;
            //     rigidbody.velocity = Vector3.zero;
            //     return;
            // }

            float squaredDistance = direction.sqrMagnitude;
            if (squaredDistance < 0.2f * 0.2f) {
                globalMovementDirectionAngle = previousAngle;
                mouseHitPoint = Vector3.zero;
                rigidbody.velocity = Vector3.zero;
                return;
            }

            int deftnessStacks = Stacks.FindAll(stack => stack.GetStackType() == Stack.StackType.DEFTNESS).Count;

            rigidbody.velocity = direction.normalized *
                                 (championStatistics.MovementSpeed * (1f + deftnessStacks / 100f));

            // Logger.Log("Velocity: " + rigidbody.velocity.magnitude, Util.Logger.Color.GREEN, this);
            lastKnownDirection = direction.normalized;
        }

        public void Stop() {
            rigidbody.velocity = Vector3.zero;
            mouseHitPoint = Vector3.zero;
        }

        private void GroundCheck() {
            // Debug.Log("Ground check");
            // Debug.DrawRay(transform.position, Vector3.down * 10f, Color.red, 0);
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundedRange, 1 << 6)) {
                // Debug.Log("Ground check hit");
                if (hit.collider.CompareTag("Ground")) {
                    // Debug.Log("Ground check hit ground");
                    grounded = true;
                }
            }
        }

        #endregion

        #region Damage & Death

        protected virtual void OnDamageTaken(float damage) {
            float fragileStacks = Stacks.FindAll(stack => stack.GetStackType() == Stack.StackType.FRAGILE).Count;
            damage = IsFragile ? damage * 1 + fragileStacks / 10 : damage;
            championStatistics.CurrentHealth -= damage;
            EventBus<ChampionDamageTakenEvent>.Raise(new ChampionDamageTakenEvent());
            if (championStatistics.CurrentHealth <= 0) {
                OnDeath();
            }
        }

        public void TakeFlatDamage(float damage) {
            OnDamageTaken(damage);
        }

        protected float CalculateDamage() {
            float damage = GetAttackDamage();

            if (Random.value < championStatistics.CriticalStrikeChance || nextAttackWillCrit) {
                damage *= (1 + championStatistics.CriticalStrikeDamage);
            }

            if (nextAttackWillCrit) nextAttackWillCrit = false;

            return Mathf.Floor(damage);
        }

        protected virtual void OnDeath() {
            // Death logic

            Destroy(gameObject);
        }

        #endregion

        #region Events

        private void OnEnemyKilledEvent(EnemyKilledEvent e) {
            championStatistics.CurrentXP += e.m_Enemy.GetXP();
            championLevelManager.CheckForLevelUp();
            EventBus<UpdateResourceBarEvent>.Raise(new UpdateResourceBarEvent("XP", championStatistics.CurrentXP,
                championLevelManager.CurrentLevelXP));
        }

        private void OnChampionAbilityChosen(ChampionAbilityChosenEvent e) {
            AAbility abilty = e.Ability;
            Logger.Log("Adding ability: " + abilty.GetType(), Logger.Color.PINK, this);
            Logger.Log("Added ability keycode: " + abilty.GetKeyCode(), Logger.Color.PINK, this);
            abilities.Add(abilty);
            abilty.Hook(this);
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

        public float GetCurrentHealth() => championStatistics.CurrentHealth;
        public float GetMaxHealth() => championStatistics.MaxHealth;

        public float GetAttackSpeed() => championStatistics.GetAttackSpeed(1 +
                                                                           (Stacks.FindAll(stack =>
                                                                                stack.GetStackType() ==
                                                                                Stack.StackType.DEFTNESS).Count /
                                                                            100f));

        public float GetAttackDamage() => championStatistics.GetAttackDamage(1 +
                                                                             (Stacks.FindAll(stack =>
                                                                                      stack.GetStackType() ==
                                                                                      Stack.StackType.OVERPOWER)
                                                                                  .Count /
                                                                              100f));

        public float GetLastAttackTime() => lastAttackTime;
        protected float GetDamageMultiplier() => damageMultiplier;
        public Rigidbody GetRigidbody() => rigidbody;
        public ChampionStatistics GetChampionStatistics() => championStatistics;
        public ChampionLevelManager GetChampionLevelManager() => championLevelManager;

        public Vector3 GetCurrentMovementDirection() {
            return rigidbody.velocity.normalized == Vector3.zero
                ? lastKnownDirection
                : rigidbody.velocity.normalized;
        }

        public float GetGlobalDirectionAngle() {
            // return the angle but instead of -180-180 i want it to be 0-360
            return globalMovementDirectionAngle < 0
                ? globalMovementDirectionAngle + 360
                : globalMovementDirectionAngle;
        }

        public int GetStackAmount(Stack.StackType stackType) {
            return Stacks.FindAll(stack => stack.GetStackType() == stackType).Count;
        }

        public void SetMouseHitPoint(Vector3 point) {
            // set the target enemy to null to stop us from auto attacking it again when we reach our destination
            currentTarget = null;
            mouseHitPoint = point;
        }

        protected void SetGlobalDirectionAngle(float angle) {
            globalMovementDirectionAngle = angle;
        }

        public void SetNextAttackWillCrit(bool b) {
            nextAttackWillCrit = b;
        }

        #endregion
    }
}