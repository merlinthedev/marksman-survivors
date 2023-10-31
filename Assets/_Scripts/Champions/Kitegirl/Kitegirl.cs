using _Scripts.Champions.Abilities;
using _Scripts.Champions.Kitegirl.Entities;
using _Scripts.Core;
using _Scripts.Entities;
using _Scripts.EventBus;
using _Scripts.Util;
using System.Collections.Generic;
using UnityEngine;
using Logger = _Scripts.Util.Logger;
using Stack = _Scripts.BuffsDebuffs.Stacks.Stack;

namespace _Scripts.Champions.Kitegirl {
    public class Kitegirl : Champion {
        [SerializeField] private KitegirlBullet bulletPrefab;
        [SerializeField] private Champion_AnimationController animationController;

        private bool attackShouldApplyDeftness = false;
        public bool shouldBurst;
        public bool shouldPierce;
        public int maxBrust;


        public override void OnAutoAttack(IDamageable damageable) {
            if (this.isCasting) return;
            if (!CanAttack) {
                if (!this.isAutoAttacking) {
                    // Queue the clicked target for our next attack
                    currentTarget = damageable;
                    // Stop moving towards the previous mouse hitpoint
                    Stop();
                }

                return;
            }

            this.mouseHitPoint = Vector3.zero;
            this.lastAttackTime = Time.time;
            Vector3 dir = damageable.GetTransform().position - transform.position;
            currentTarget = damageable;

            AutoAttackStarted();

            // Set to Vector3.zero to stop us from moving towards the previous mouse hitpoint


            SetGlobalDirectionAngle(Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg);
            SetLastAttackDirection(dir.normalized);
            // Utilities.InvokeDelayed(() => { SetCanMove(true); }, 0.1f, this);
            this.rigidbody.velocity = Vector3.zero;

            if (attackShouldApplyDeftness) {
                AddStacks(1, Stack.StackType.DEFTNESS);
            }

            // Logger.Log("Deftness stacks: " + GetStackAmount(Stack.StackType.DEFTNESS), Logger.Color.YELLOW, this);
            // Logger.Log("Overpower stacks: " + GetStackAmount(Stack.StackType.OVERPOWER), Logger.Color.YELLOW, this);

            Shoot();


            animationController.Attack();
            this.isAutoAttacking = true;
            EventBus<ChampionAbilityUsedEvent>.Raise(new ChampionAbilityUsedEvent(KeyCode.Mouse0, GetAttackSpeed()));
        }


        public void SmokeScreenPushBack(float force, float yForceOffset, Vector3 mousePosition) {
            // take the direction from the mouse position to the champion
            Vector3 pushbackDirection =
                (transform.position - new Vector3(mousePosition.x, yForceOffset, mousePosition.z)).normalized;

            // Debug.DrawRay(transform.position, pushbackDirection, Color.yellow);
            // Debug.DrawLine(transform.position, mousePosition, Color.yellow, 1f);

            // Debug.Log("<color=yellow>Pushback direction: " + pushbackDirection + "</color>", this);

            this.rigidbody.AddForce(pushbackDirection * force, ForceMode.Impulse);
        }


        public override void DealDamage(IDamageable damageable, float damage, DamageType damageType,
            bool shouldInvoke = true) {
            if (attackShouldApplyDeftness) {
                AddStacks(1, Stack.StackType.DEFTNESS);
            }

            Logger.Log("DamageType: " + damageType, Logger.Color.YELLOW, this);

            // if (damageType == DamageType.NON_BASIC && IsReady) {
            //     Logger.Log("Dealing more damage because of rhythm", Logger.Color.YELLOW, this);
            //     base.DealDamage(damageable, damage * 1.1f, damageType, shouldInvoke);
            //     return;
            // }
            if (IsReady) {
                Logger.Log("We were ready", this);
                if (damageType.Equals(DamageType.NON_BASIC)) {
                    Logger.Log("Dealing more damage because of rhythm", Logger.Color.YELLOW, this);
                    base.DealDamage(damageable, damage * 1.1f, damageType, shouldInvoke);
                    return;
                }
            }

            base.DealDamage(damageable, damage, damageType, shouldInvoke);
        }

        protected override void Update() {
            base.Update(); // this is important, even tho the editor says it's not...
        }

        private void Start() {
            base.Start();
            foreach (Ability ability in this.abilities) {
                ability.Hook(this);
            }

            EventBus<ChampionAbilitiesHookedEvent>.Raise(new ChampionAbilitiesHookedEvent()); // TODO: REFACTOR
        }

        public void Shoot() {
            Vector3 dir = currentTarget.GetTransform().position - transform.position;
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;


            if (shouldBurst) {
                Utilities.DelayedForLoop(maxBrust, 0.2f, () => {
                    KitegirlBullet kitegirlBullet =
                        Instantiate(bulletPrefab, transform.position, Quaternion.Euler(90, angle, 0));
                    kitegirlBullet.SetSourceEntity(this);
                    kitegirlBullet.SetTarget(currentTarget);
                    kitegirlBullet.SetDamage(CalculateDamage());
                    kitegirlBullet.ShouldPierce(shouldPierce);
                    kitegirlBullet.Init(BulletHit);
                }, this);
            } else {
                KitegirlBullet kitegirlBullet =
                    Instantiate(bulletPrefab, transform.position, Quaternion.Euler(90, angle, 0));
                kitegirlBullet.SetSourceEntity(this);
                kitegirlBullet.SetTarget(currentTarget);
                kitegirlBullet.SetDamage(CalculateDamage());
                kitegirlBullet.Init(BulletHit);
            }
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
            DealDamage(damageable, CalculateDamage() * Mathf.Pow(0.5f, index), DamageType.BASIC);
        }

        private void AnimateBulletBounce(List<IDamageable> targets, int index) {
            Vector3 dir = targets[index].GetTransform().position - targets[index - 1].GetTransform().position;
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

            KitegirlBullet bouncingBullet = Instantiate(bulletPrefab, targets[index - 1].GetTransform().position,
                Quaternion.Euler(90, angle, 0));
            bouncingBullet.IsFake();
            bouncingBullet.SetTarget(targets[index]);
        }

        public Champion_AnimationController GetAnimator() => animationController;

        public void SetAttackDeftnessApply(bool value) {
            attackShouldApplyDeftness = value;
        }

        public void EnableStuffAfterAttack() {
            // Logger.Log("THIS METHOD IS NOT IMPLEMENTED ANYMORE PLS FIX :D", Logger.Color.RED, this);
            this.isAutoAttacking = false;
        }
    }
}