using Champions.Abilities;
using Champions.Kitegirl.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Champions.Kitegirl.Abilities {
    public class KitegirlW1 : AAbility {
        [FormerlySerializedAs("m_GrenadePrefab")] [SerializeField] private KitegirlGrenade grenadePrefab;

        private float attackDamageRatio = 0.6f; // 0.6f => 60% of AD

        public override void OnUse() {
            if (IsOnCooldown()) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.gameObject.CompareTag("Ground") || hit.collider.gameObject.CompareTag("Enemy")) {
                    Vector3 point = hit.point;
                    point.y = this.champion.transform.position.y - 0.2f;

                    // Debug.Log("Point: " + point, this);

                    if (DistanceCheck(point)) {
                        KitegirlGrenade grenade =
                            Instantiate(grenadePrefab, this.champion.transform.position, grenadePrefab.transform.rotation);
                        grenade.SetDamage(this.champion.GetAttackDamage() * attackDamageRatio);
                        grenade.OnThrow(point, champion);

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
        protected internal override void DeductFromCooldown(float timeToDeduct) {
            base.DeductFromCooldown(timeToDeduct);
        }
    }
}