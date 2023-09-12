using Champions.Kitegirl.Entities;
using Events;
using UnityEngine;
using Util;
using Logger = Util.Logger;

namespace Champions.Kitegirl {
    public class Kitegirl : Champion {
        [SerializeField] private KitegirlBullet m_BulletPrefab;
        [SerializeField] private Champion_AnimationController m_AnimationController;

        private bool m_AutoAttackShouldChain = false;
        private bool m_IsDashing = false;
        private bool m_HasUltimateActive = false;

        private int m_RecurseCount = 0;
        private int m_MaxRecurseCount = 0;

        [SerializeField] private int m_MaxChainCount = 3;

        private int m_CurrentChainCount = 0;

        [SerializeField] private float m_DashSpeed = 20f;

        public override void OnAutoAttack(Collider collider) {
            if (!CanAttack) {
                if (!m_IsAutoAttacking) {
                    // Queue the clicked target for our next attack
                    m_CurrentTarget = EnemyManager.GetInstance().GetEnemy(collider);
                    // Stop moving towards the previous mouse hitpoint
                    Stop();
                }

                return;
            }

            // Set to Vector3.zero to stop us from moving towards the previous mouse hitpoint
            m_MouseHitPoint = Vector3.zero;

            m_LastAttackTime = Time.time;
            Vector3 dir = collider.transform.position - transform.position;
            m_CurrentTarget = EnemyManager.GetInstance().GetEnemy(collider);

            SetGlobalDirectionAngle(Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg);
            // Utilities.InvokeDelayed(() => { SetCanMove(true); }, 0.1f, this);
            // TODO: Instead of 0.1f, either anim event or smth else to determnie when the attack is over
            m_Rigidbody.velocity = Vector3.zero;

            ShootBullet_Recursive(m_HasUltimateActive,
                new Vector3(collider.transform.position.x, transform.position.y, collider.transform.position.z));
            m_AnimationController.Attack();
            m_IsAutoAttacking = true;
            EventBus<ChampionAbilityUsedEvent>.Raise(new ChampionAbilityUsedEvent(KeyCode.Mouse0, GetAttackSpeed()));
        }

        public override void OnAbility(KeyCode keyCode) {
            AAbility ability = this.m_Abilities.Find(ability => ability.GetKeyCode() == keyCode);

            if (ability != null) {
                ability.OnUse();
            }
            else {
                // Debug.Log("Ability not found");
            }
        }

        public void ActivateUltimate(float duration, float burstAmount, float slowAmount) {
            m_HasUltimateActive = true;
            m_MaxRecurseCount = (int)burstAmount;
            ApplyDebuff(Debuff.CreateDebuff(this, Debuff.DebuffType.Slow, duration, slowAmount));
        }

        public void DeactivateUltimate() {
            m_HasUltimateActive = false;
        }

        public void TryReduceECooldown() {
            AAbility kitegirlE = m_Abilities.Find(ability => ability.GetKeyCode() == KeyCode.E);
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

            m_Rigidbody.AddForce(pushbackDirection * force, ForceMode.Impulse);
        }

        protected override void OnMove() {
            if (!m_IsDashing) {
                base.OnMove();
            }
            else {
                m_Rigidbody.velocity = GetCurrentMovementDirection() * m_DashSpeed;
            }
        }

        protected override void Update() {
            base.Update(); // this is important, even tho the editor says it's not...
        }

        private void Start() {
            base.Start();
            foreach (AAbility ability in m_Abilities) {
                ability.Hook(this);
            }

            EventBus<ChampionAbilitiesHookedEvent>.Raise(new ChampionAbilitiesHookedEvent()); // TODO: REFACTOR
        }


        private void ShootBullet_Recursive(bool shouldCallRecursive, Vector3 target) {
            m_RecurseCount++;

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
            bullet.SetShouldChain(m_AutoAttackShouldChain);
            bullet.SetTarget(target + randomBulletSpread);
            bullet.SetDamage(CalculateDamage());


            if (m_AutoAttackShouldChain) {
                m_CurrentChainCount++;
                // Debug.Log("Chain count: " + m_CurrentChainCount + " / " + m_MaxChainCount + "");
                if (m_CurrentChainCount >= m_MaxChainCount) {
                    m_CurrentChainCount = 0;
                    m_AutoAttackShouldChain = false;
                    // Debug.Log("Chain count reset");
                }
            }

            if (shouldCallRecursive && m_RecurseCount < m_MaxRecurseCount) {
                Utilities.InvokeDelayed(() => { ShootBullet_Recursive(true, target); }, 0.05f, this);
            }
            else {
                m_RecurseCount = 0;
            }
        }


        public void SetAutoAttackChain(bool b) {
            m_AutoAttackShouldChain = b;
        }

        public void SetIsDashing(bool p0) {
            m_IsDashing = p0;
        }

        public void EnableStuffAfterAttack() {
            Logger.Log("THIS METHOD IS NOT IMPLEMENTED ANYMORE PLS FIX :D", Logger.Color.RED, this);
            m_IsAutoAttacking = false;
        }

        public bool HasUltimateActive() {
            return m_HasUltimateActive;
        }
    }
}