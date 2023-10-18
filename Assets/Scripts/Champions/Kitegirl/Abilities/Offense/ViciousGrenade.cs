using Champions.Abilities;
using Champions.Kitegirl.Entities;
using Core;
using UnityEngine;

namespace Champions.Kitegirl.Abilities.Offense {
    public class ViciousGrenade : Ability {
        [SerializeField] private KitegirlGrenade grenadePrefab;

        [SerializeField] private float attackDamageRatio = 1f; // 0.6f => 60% of AD

        public override void OnUse() {
            if (!CanAfford()) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.gameObject.CompareTag("Ground") || hit.collider.gameObject.CompareTag("Enemy")) {
                    Vector3 point = hit.point;
                    point.y = this.champion.transform.position.y - 0.2f;

                    // Debug.Log("Point: " + point, this);

                    if (DistanceCheck(point)) {
                        KitegirlGrenade grenade =
                            Instantiate(grenadePrefab,
                                new Vector3(this.champion.transform.position.x, 0.5f,
                                    this.champion.transform.position.z),
                                grenadePrefab.transform.rotation);
                        grenade.SetDamage(this.champion.GetAttackDamage() * attackDamageRatio);
                        grenade.OnThrow(new Vector3(point.x, 0.5f, point.z), champion);

                        DamageableManager.GetInstance().AddDamageable(grenade);

                        base.OnUse();
                    }
                }
            }
        }


        // WE NEED THIS FUNCTION DO NOT DELETE
        protected override void ResetCooldown() {
            base.ResetCooldown();
        }

        // WE NEED THIS FUNCTION DO NOT DELETE
        protected override void DeductFromCooldown(float timeToDeduct) {
            base.DeductFromCooldown(timeToDeduct);
        }
    }
}