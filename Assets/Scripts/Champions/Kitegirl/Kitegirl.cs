using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BuffsDebuffs;
using Champions.Abilities;
using Champions.Kitegirl.Entities;
using Core;
using Entities;
using EventBus;
using UnityEngine;
using UnityEngine.Serialization;
using Util;
using Stack = BuffsDebuffs.Stacks.Stack;

namespace Champions.Kitegirl {
    public class Kitegirl : Champion {
        [SerializeField] private KitegirlBullet bulletPrefab;
        [SerializeField] private Champion_AnimationController animationController;

        private bool autoAttackShouldChain = false;
        private bool hasUltimateActive = false;
        private bool attackShouldApplyDeftness = false;

        private int recurseCount = 0;
        private int maxRecurseCount = 0;

        [SerializeField] private int maxChainCount = 3;

        private int currentChainCount = 0;


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

            mouseHitPoint = Vector3.zero;
            lastAttackTime = Time.time;
            Vector3 dir = damageable.GetTransform().position - transform.position;
            currentTarget = damageable;

            AutoAttackStarted();

            // Set to Vector3.zero to stop us from moving towards the previous mouse hitpoint


            SetGlobalDirectionAngle(Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg);
            SetLastAttackDirection(dir.normalized);
            // Utilities.InvokeDelayed(() => { SetCanMove(true); }, 0.1f, this);
            rigidbody.velocity = Vector3.zero;

            if (attackShouldApplyDeftness) {
                AddStacks(1, Stack.StackType.DEFTNESS);
            }

            // Logger.Log("Deftness stacks: " + GetStackAmount(Stack.StackType.DEFTNESS), Logger.Color.YELLOW, this);
            // Logger.Log("Overpower stacks: " + GetStackAmount(Stack.StackType.OVERPOWER), Logger.Color.YELLOW, this);

            Shoot();


            animationController.Attack();
            isAutoAttacking = true;
            EventBus<ChampionAbilityUsedEvent>.Raise(new ChampionAbilityUsedEvent(KeyCode.Mouse0, GetAttackSpeed()));
        }

        public void ActivateUltimate(float duration, float burstAmount, float slowAmount) {
            hasUltimateActive = true;
            maxRecurseCount = (int)burstAmount;
            ApplyDebuff(Debuff.CreateDebuff(this, this, Debuff.DebuffType.SLOW, duration, slowAmount));
        }

        public void DeactivateUltimate() {
            hasUltimateActive = false;
        }

        [Obsolete("Not in use anymore.")]
        public void TryReduceECooldown() {
            // Ability kitegirlE = abilities.Find(ability => ability.GetKeyCode() == KeyCode.E);
            // if (kitegirlE == null) return;
            // if (kitegirlE.IsOnCooldown()) {
            //     kitegirlE.DeductFromCooldown(kitegirlE.GetAbilityCooldown() * 0.02f); // 2% of cooldown 
            // }
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
            foreach (Ability ability in abilities) {
                ability.Hook(this);
            }

            EventBus<ChampionAbilitiesHookedEvent>.Raise(new ChampionAbilitiesHookedEvent()); // TODO: REFACTOR
        }

        public void Shoot() {
            Vector3 dir = currentTarget.GetTransform().position - transform.position;
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;


            KitegirlBullet bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(90, angle, 0));
            bullet.SetSourceEntity(this);
            bullet.SetTarget(currentTarget);
            bullet.SetDamage(CalculateDamage());
            bullet.Init(BulletHit);
        }

        public void Bounce(int bounces, float timeBetweenBounces, IDamageable initialTarget) {
            List<IDamageable> alreadyHit = new List<IDamageable>();
            alreadyHit.Add(initialTarget);
            int index = 1;

            for (int i = 1; i <= bounces; i++) {
                IDamageable damageable = DamageableManager.GetInstance()
                    .GetClosestDamageable(alreadyHit[i - 1].GetTransform().position, alreadyHit);
                alreadyHit.Add(damageable);


                // invoke the bounce after the delay
                var i1 = i;
                Utilities.InvokeDelayed(() => {
                    // Debug.Log("Bouncing to " + damageable, this);
                    BounceTo(damageable, i1);

                }, timeBetweenBounces * i, this);
                
                Utilities.InvokeDelayed(() => {
                    AnimateBulletBounce(alreadyHit, index);
                    index++;
                }, timeBetweenBounces * (i - 1), this);
            }
        }

        private void BounceTo(IDamageable damageable, int index) {
            DealDamage(damageable, CalculateDamage() * Mathf.Pow(0.5f, index));
        }
        private void AnimateBulletBounce(List<IDamageable> targets, int index) {
            Vector3 dir = targets[index].GetTransform().position - targets[index - 1].GetTransform().position;
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            
            KitegirlBullet bouncingBullet = Instantiate(bulletPrefab, targets[index - 1].GetTransform().position, Quaternion.Euler(90, angle, 0));
            bouncingBullet.IsFake();
            bouncingBullet.SetTarget(targets[index]);
        }

        public void SetAutoAttackChain(bool b) {
            autoAttackShouldChain = b;
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