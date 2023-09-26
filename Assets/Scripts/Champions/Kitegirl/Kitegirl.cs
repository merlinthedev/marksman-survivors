using BuffsDebuffs;
using BuffsDebuffs.Stacks;
using Champions.Abilities;
using Champions.Kitegirl.Entities;
using Entities;
using EventBus;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace Champions.Kitegirl {
    public class Kitegirl : Champion {
        [SerializeField] private KitegirlBullet m_BulletPrefab;
        [SerializeField] private Champion_AnimationController m_AnimationController;

        private bool autoAttackShouldChain = false;
        private bool isDashing = false;
        private bool hasUltimateActive = false;
        private bool attackShouldApplyDeftness = false;

        private int recurseCount = 0;
        private int maxRecurseCount = 0;

        [SerializeField] private int maxChainCount = 3;

        private int currentChainCount = 0;

        [SerializeField] private float dashSpeed = 20f;

        public override void OnAutoAttack(IDamageable damageable) {
            if (!CanAttack) {
                if (!isAutoAttacking) {
                    // Queue the clicked target for our next attack
                    currentTarget = damageable;
                    // Stop moving towards the previous mouse hitpoint
                    Stop();
                }

                return;
            }

            // Set to Vector3.zero to stop us from moving towards the previous mouse hitpoint
            mouseHitPoint = Vector3.zero;

            lastAttackTime = Time.time;
            Vector3 dir = damageable.GetTransform().position - transform.position;
            currentTarget = damageable;


            SetGlobalDirectionAngle(Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg);
            // Utilities.InvokeDelayed(() => { SetCanMove(true); }, 0.1f, this);
            rigidbody.velocity = Vector3.zero;

            if (attackShouldApplyDeftness) {
                AddStacks(1, Stack.StackType.DEFTNESS);
            }

            // Logger.Log("Deftness stacks: " + GetStackAmount(Stack.StackType.DEFTNESS), Logger.Color.YELLOW, this);
            // Logger.Log("Overpower stacks: " + GetStackAmount(Stack.StackType.OVERPOWER), Logger.Color.YELLOW, this);

            ShootBullet_Recursive(hasUltimateActive,
                new Vector3(damageable.GetTransform().position.x, transform.position.y,
                    damageable.GetTransform().position.z));
            m_AnimationController.Attack();
            isAutoAttacking = true;
            EventBus<ChampionAbilityUsedEvent>.Raise(new ChampionAbilityUsedEvent(KeyCode.Mouse0, GetAttackSpeed()));
        }

        public override void OnAbility(KeyCode keyCode) {
            AAbility ability = abilities.Find(ability => ability.GetKeyCode() == keyCode);

            if (ability != null) {
                ability.OnUse();
            }
            else {
                Debug.Log("Ability not found, please check the KeyCode");
                // log the keycode
                Debug.Log("Keycode: " + keyCode);
            }
        }

        public void ActivateUltimate(float duration, float burstAmount, float slowAmount) {
            hasUltimateActive = true;
            maxRecurseCount = (int)burstAmount;
            ApplyDebuff(Debuff.CreateDebuff(this, this, Debuff.DebuffType.SLOW, duration, slowAmount));
        }

        public void DeactivateUltimate() {
            hasUltimateActive = false;
        }

        public void TryReduceECooldown() {
            AAbility kitegirlE = abilities.Find(ability => ability.GetKeyCode() == KeyCode.E);
            if (kitegirlE == null) return;
            if (kitegirlE.IsOnCooldown()) {
                kitegirlE.DeductFromCooldown(kitegirlE.GetAbilityCooldown() * 0.02f); // 2% of cooldown 
            }
        }

        public void SmokeScreenPushBack(float force, float yForceOffset, Vector3 mousePosition) {
            // take the direction from the mouse position to the champion
            Vector3 pushbackDirection =
                (transform.position - new Vector3(mousePosition.x, yForceOffset, mousePosition.z)).normalized;

            // Debug.DrawRay(transform.position, pushbackDirection, Color.yellow);
            // Debug.DrawLine(transform.position, mousePosition, Color.yellow, 1f);

            // Debug.Log("<color=yellow>Pushback direction: " + pushbackDirection + "</color>", this);

            rigidbody.AddForce(pushbackDirection * force, ForceMode.Impulse);
        }

        protected override void OnMove() {
            if (!isDashing) {
                base.OnMove();
            }
            else {
                rigidbody.velocity = GetCurrentMovementDirection() * dashSpeed;
            }
        }

        public override void DealDamage(IDamageable damageable, float damage) {
            if (attackShouldApplyDeftness) {
                AddStacks(1, Stack.StackType.DEFTNESS);
            }

            base.DealDamage(damageable, damage);
        }

        protected override void Update() {
            base.Update(); // this is important, even tho the editor says it's not...
        }

        private void Start() {
            base.Start();
            foreach (AAbility ability in abilities) {
                ability.Hook(this);
            }

            EventBus<ChampionAbilitiesHookedEvent>.Raise(new ChampionAbilitiesHookedEvent()); // TODO: REFACTOR
        }


        private void ShootBullet_Recursive(bool shouldCallRecursive, Vector3 target) {
            recurseCount++;

            Vector3 randomBulletSpread = new Vector3(
                Random.Range(-0.1f, 0.1f),
                0,
                Random.Range(-0.1f, 0.1f)
            );

            // ABullet aBullet = Instantiate(mABulletPrefab, transform.position, Quaternion.identity);
            // aBullet.SetShouldChain(m_AutoAttackShouldChain);
            // aBullet.SetTarget(target + randomBulletSpread);
            // aBullet.SetDamage(m_Damage);

            KitegirlBullet bullet = Instantiate(m_BulletPrefab, transform.position, Quaternion.identity);
            bullet.SetSourceEntity(this);
            bullet.SetShouldChain(autoAttackShouldChain);
            bullet.SetTarget(target + randomBulletSpread);
            bullet.SetDamage(CalculateDamage());


            if (autoAttackShouldChain) {
                currentChainCount++;
                // Debug.Log("Chain count: " + m_CurrentChainCount + " / " + m_MaxChainCount + "");
                if (currentChainCount >= maxChainCount) {
                    currentChainCount = 0;
                    autoAttackShouldChain = false;
                    // Debug.Log("Chain count reset");
                }
            }

            if (shouldCallRecursive && recurseCount < maxRecurseCount) {
                Utilities.InvokeDelayed(() => { ShootBullet_Recursive(true, target); }, 0.05f, this);
            }
            else {
                recurseCount = 0;
            }
        }


        public void SetAutoAttackChain(bool b) {
            autoAttackShouldChain = b;
        }

        public void SetIsDashing(bool p0) {
            isDashing = p0;
        }

        public void SetAttackDeftnessApply(bool value) {
            attackShouldApplyDeftness = value;
        }

        public void EnableStuffAfterAttack() {
            // Logger.Log("THIS METHOD IS NOT IMPLEMENTED ANYMORE PLS FIX :D", Logger.Color.RED, this);
            isAutoAttacking = false;
        }

        public bool HasUltimateActive() {
            return hasUltimateActive;
        }
    }
}