using System;
using System.Collections.Generic;
using BuffsDebuffs;
using BuffsDebuffs.Stacks;
using Champions.Abilities;
using Enemies;
using Entities;
using EventBus;
using UnityEngine;
using Util;
using Debug = UnityEngine.Debug;
using Logger = Util.Logger;
using Random = UnityEngine.Random;

namespace Champions {
    public abstract class Champion : AbilityHolder, IDebuffable, IDamager, IStackableLivingEntity {
        #region Properties

        [Header("References")]
        [SerializeField] protected Rigidbody rigidbody;

        [SerializeField] private Collider collider;

        [Header("Stats")]
        [SerializeField] protected ChampionStatistics championStatistics;

        private float lastManaRegenerationTime = 0f;
        private float lastHealthRegenerationTime = 0f;
        private ChampionLevelManager championLevelManager;

        [Header("Movement")]
        [SerializeField] protected Vector3 mouseHitPoint;

        private Vector3 lastKnownDirection = Vector3.zero;

        [SerializeField] protected bool grounded = false;

        [SerializeField] private float groundedRange = 1.4f;

        private float globalMovementDirectionAngle = 0f;
        private float previousAngle = 0f;
        public bool IsMoving => rigidbody.velocity.magnitude > 0.001f;


        public IDamageable currentTarget { get; set; }

        [Header("Attack")]
        private bool nextAttackWillCrit = false;

        protected float lastAttackTime = 0f;
        private float damageMultiplier = 1f;

        private Dodge dodge;
        [SerializeField] private float dashCooldown = 20f;
        [SerializeField] private float dashDuration = 0.2f;
        [SerializeField] private float dashSpeed = 20f;
        private bool isDashing = false;

        private bool shouldMove => mouseHitPoint != Vector3.zero;

        protected bool isAutoAttacking = false;

        protected bool CanAttack {
            get => !isAutoAttacking && Time.time > lastAttackTime + (1f / GetAttackSpeed());
        }

        [Header("Cheats")]
        [SerializeField] private bool isInvincible = false;

        //Buff/Debuff
        public List<Debuff> Debuffs { get; } = new();
        public List<Stack> Stacks { get; } = new();
        public bool IsFragile { get; }

        private MovementDirection CurrentMovementDirection {
            get {
                switch (GetGlobalDirectionAngle()) {
                    case float n when n > 0 && n < 90:
                        return MovementDirection.NORTH;
                    case float n when n >= 90 && n < 180:
                        return MovementDirection.EAST;
                    case float n when n >= 180 && n < 270:
                        return MovementDirection.SOUTH;
                    case float n when n >= 270 && n < 360:
                        return MovementDirection.WEST;
                    default:
                        return MovementDirection.ZERO;
                }
            }
        }

        private bool movingInSameDirectionForTooLong = false;
        private float lastMovementDirectionChangeTime = 0f;


        [SerializeField] private Vector4 directionTracker = Vector4.zero;

        #endregion

        public event Action<IDamageable> OnAutoAttackEnded;
        public event Action<IDamageable> OnAutoAttackStarted;

        public void AutoAttackEnded(IDamageable t) {
            OnAutoAttackEnded?.Invoke(t);
        }

        public void AutoAttackStarted() {
            OnAutoAttackStarted?.Invoke(currentTarget);
        }


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

            dodge = new Dodge(dashCooldown);
        }

        protected virtual void FixedUpdate() {
            GroundCheck();

            if (grounded) {
                OnMove();
            }

            ChangeMovementInfluence();

            grounded = false;
        }

        protected virtual void Update() {
            if (!IsMoving && currentTarget != null) {
                OnAutoAttack(currentTarget);
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

            if (Input.GetKeyDown(KeyCode.L)) {
                championStatistics.CurrentXP += 100;
                championLevelManager.CheckForLevelUp();
            }

            // Logger.Log("Amount of deftness stacks: " + GetStackAmount(Stack.StackType.DEFTNESS), Logger.Color.GREEN,
            //     this);
            // Logger.Log("Amount of overpower stacks: " + GetStackAmount(Stack.StackType.OVERPOWER), Logger.Color.GREEN,
            //     this);
        }

        #endregion

        #region Abstract Methods

        public abstract void OnAutoAttack(IDamageable damageable);

        public void OnAbility(Ability ability) {
            ability.OnUse();
        }

        #endregion

        #region Debuffs

        public void RemoveDebuff(Debuff debuff) {
            Debuffs.Remove(debuff);
        }

        public void ApplyDebuff(Debuff debuff) {
            Debuffs.Add(debuff);

            switch (debuff.GetDebuffType()) {
                case Debuff.DebuffType.SLOW:
                    ApplySlow(debuff);
                    break;
                case Debuff.DebuffType.BURN:
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
            EventBus<ChangeStackUIEvent>.Raise(new ChangeStackUIEvent(stack.GetStackType(),
                GetStackAmount(stack.GetStackType()), true));
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

            EventBus<ChangeStackUIEvent>.Raise(new ChangeStackUIEvent(Stack.StackType.FRAGILE,
                GetStackAmount(Stack.StackType.FRAGILE), true));
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

            EventBus<ChangeStackUIEvent>.Raise(new ChangeStackUIEvent(Stack.StackType.DEFTNESS,
                GetStackAmount(Stack.StackType.DEFTNESS), true));
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

            EventBus<ChangeStackUIEvent>.Raise(new ChangeStackUIEvent(Stack.StackType.OVERPOWER,
                GetStackAmount(Stack.StackType.OVERPOWER), true));
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

        public void RequestMovement(Vector3 target, float distance = 0.2f, Action callback = null) {
            mouseHitPoint = target;
            currentTarget = null;
            movementCallback = callback;
            distanceBeforeCallback = distance;
        }

        private Action movementCallback = null;
        private float distanceBeforeCallback = 0.2f;

        protected virtual void OnMove() {
            if (!shouldMove || isDashing) {
                return;
            }

            // Debug.Log("Moving");
            Vector3 direction = mouseHitPoint - transform.position;

            previousAngle = globalMovementDirectionAngle;
            globalMovementDirectionAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            // Logger.Log("globalMovementDirectionAngle: " + globalMovementDirectionAngle, Logger.Color.GREEN, this);

            float squaredDistance = direction.sqrMagnitude;
            if (squaredDistance < distanceBeforeCallback * distanceBeforeCallback) {
                globalMovementDirectionAngle = previousAngle;
                mouseHitPoint = Vector3.zero;
                rigidbody.velocity = Vector3.zero;
                movementCallback?.Invoke();
                movementCallback = null;
                return;
            }

            int deftnessStacks = Stacks.FindAll(stack => stack.GetStackType() == Stack.StackType.DEFTNESS).Count;

            rigidbody.velocity =
                direction.normalized * (championStatistics.MovementSpeed * (1f + deftnessStacks / 100f));

            // Logger.Log("Velocity: " + rigidbody.velocity.magnitude, Util.Logger.Color.GREEN, this);
            lastKnownDirection = direction.normalized;
        }

        public void OnDash() {
            if (dodge.IsOnCooldown()) {
                Logger.Log("Dash is on cooldown.", this);
                return;
            }

            bool isMovingPreDash = IsMoving;
            isDashing = true;
            float angle = Utilities.GetGlobalAngleFromDirection(this);

            rigidbody.useGravity = false;
            collider.isTrigger = true;

            rigidbody.velocity = Utilities.GetPointToMouseDirection(transform.position) * dashSpeed;

            Logger.Log("Angle: " + angle, Logger.Color.RED, this);


            Utilities.InvokeDelayed(() => {
                isDashing = false;
                collider.isTrigger = false;
                rigidbody.useGravity = true;
                if (angle > 45 || angle < -45) {
                    Stop();
                    ResetCurrentTarget();
                } else {
                    if (!isMovingPreDash) {
                        Stop();
                    }
                }
            }, dashDuration, this);

            dodge.StartCooldown();
            EventBus<ChampionAbilityUsedEvent>.Raise(new ChampionAbilityUsedEvent(KeyCode.Space, dodge.GetCooldown()));
        }

        private const float increaseValue = 0.008f;
        private const float decreaseValue = 0.009f;

        private void ChangeMovementInfluence() {
            if (CurrentMovementDirection == MovementDirection.ZERO) return;

            if (CurrentMovementDirection == MovementDirection.NORTH) {
                directionTracker.x += increaseValue;

                if (directionTracker.y > 0) directionTracker.y -= decreaseValue;
                if (directionTracker.z > 0) directionTracker.z -= decreaseValue;
                if (directionTracker.w > 0) directionTracker.w -= decreaseValue;
            } else if (CurrentMovementDirection == MovementDirection.EAST) {
                directionTracker.y += increaseValue;

                if (directionTracker.x > 0) directionTracker.x -= decreaseValue;
                if (directionTracker.z > 0) directionTracker.z -= decreaseValue;
                if (directionTracker.w > 0) directionTracker.w -= decreaseValue;
            } else if (CurrentMovementDirection == MovementDirection.SOUTH) {
                directionTracker.z += increaseValue;

                if (directionTracker.x > 0) directionTracker.x -= decreaseValue;
                if (directionTracker.y > 0) directionTracker.y -= decreaseValue;
                if (directionTracker.w > 0) directionTracker.w -= decreaseValue;
            } else if (CurrentMovementDirection == MovementDirection.WEST) {
                directionTracker.w += increaseValue;

                if (directionTracker.x > 0) directionTracker.x -= decreaseValue;
                if (directionTracker.y > 0) directionTracker.y -= decreaseValue;
                if (directionTracker.z > 0) directionTracker.z -= decreaseValue;
            }

            directionTracker = Utilities.ClampVector4(directionTracker, 0, 1);
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

        private void OnDamageTaken(float damage) {
            if (isInvincible) return;
            damage = CalculateIncomingDamage(damage);
            // Debug.Log("With value " + damage);
            championStatistics.CurrentHealth -= damage;
            EventBus<ChampionDamageTakenEvent>.Raise(new ChampionDamageTakenEvent());
            if (championStatistics.CurrentHealth <= 0) {
                OnDeath();
            }
        }

        public void TakeFlatDamage(float damage) {
            if (isInvincible) return;
            // Debug.Log("Taking damage");
            OnDamageTaken(damage);
        }

        public float CalculateIncomingDamage(float damage) {
            float fragileStacks = Stacks.FindAll(stack => stack.GetStackType() == Stack.StackType.FRAGILE).Count;
            damage = IsFragile ? damage * 1 + fragileStacks / 100f : damage;

            return damage;
        }

        public void ToggleInvincibility() {
            isInvincible = !isInvincible;
        }

        protected float CalculateDamage() {
            float damage = GetAttackDamage();

            if (Random.value < championStatistics.CriticalStrikeChance || nextAttackWillCrit) {
                damage *= (1 + championStatistics.CriticalStrikeDamage);
            }

            if (nextAttackWillCrit) nextAttackWillCrit = false;

            return Mathf.Floor(damage);
        }

        public virtual void DealDamage(IDamageable damageable, float damage) {
            damageable.TakeFlatDamage(damage);
        }

        public void OnDeath() {
            // Death logic

            EventBus<LoadSceneEvent>.Raise(new LoadSceneEvent("D Hub"));
        }

        #endregion

        #region Events

        private void OnEnemyKilledEvent(EnemyKilledEvent e) {
            championStatistics.CurrentXP += e.enemy.GetXP();
            championLevelManager.CheckForLevelUp();
            EventBus<UpdateResourceBarEvent>.Raise(new UpdateResourceBarEvent("XP", championStatistics.CurrentXP,
                championLevelManager.CurrentLevelXP));

            // if the enemy that was killed was the instance of currentTarget, set currentTarget to null
            if ((Enemy)currentTarget == e.enemy) {
                currentTarget = null;
            }
        }

        private void OnChampionAbilityChosen(ChampionAbilityChosenEvent e) {
            Ability abilty = e.Ability;
            Logger.Log("Adding ability: " + abilty.GetType(), Logger.Color.PINK, this);
            // Logger.Log("Added ability keycode: " + abilty.GetKeyCode(), Logger.Color.PINK, this);
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
                                                                                 .Count / 100f));

        public float GetLastAttackTime() => lastAttackTime;
        protected float GetDamageMultiplier() => damageMultiplier;
        public Rigidbody GetRigidbody() => rigidbody;
        public ChampionStatistics GetChampionStatistics() => championStatistics;
        public ChampionLevelManager GetChampionLevelManager() => championLevelManager;

        public IDamageable GetCurrentTarget() => currentTarget;

        public Vector3 GetCurrentMovementDirection() {
            // Logger.Log("WHERET HE FUCK IS MY GETTER?XD ", Logger.Color.RED, this);
            // Logger.Log("Velocity: " + rigidbody.velocity.normalized, Logger.Color.GREEN, this);
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

        public Vector4 GetMovementData() {
            return directionTracker;
        }

        public int GetStackAmount(Stack.StackType stackType) {
            return Stacks.FindAll(stack => stack.GetStackType() == stackType).Count;
        }

        public Transform GetTransform() {
            return gameObject.transform;
        }

        public Dodge GetDodge() {
            return dodge;
        }

        protected void SetGlobalDirectionAngle(float angle) {
            globalMovementDirectionAngle = angle;
        }

        public void SetNextAttackWillCrit(bool b) {
            nextAttackWillCrit = b;
        }

        public void ResetCurrentTarget() {
            currentTarget = null;
        }

        #endregion

        public enum MovementDirection {
            ZERO,
            NORTH,
            EAST,
            SOUTH,
            WEST
        }
    }
}