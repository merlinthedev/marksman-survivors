using Champions.Abilities;
using Champions.Kitegirl.Entities;
using UnityEngine;

namespace Champions.Kitegirl.Abilities {
    public class KitegirlW1 : AAbility {
        [SerializeField] private KitegirlGrenade m_GrenadePrefab;

        private float m_AttackDamageRatio = 0.6f; // 0.6f => 60% of AD

        public override void OnUse() {
            if (IsOnCooldown()) return;
            base.OnUse();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.gameObject.CompareTag("Ground") || hit.collider.gameObject.CompareTag("Enemy")) {
                    Vector3 point = hit.point;
                    point.y = this.m_Champion.transform.position.y - 0.2f;

                    // Debug.Log("Point: " + point, this);

                    if (DistanceCheck(point)) {
                        KitegirlGrenade grenade =
                            Instantiate(m_GrenadePrefab, this.m_Champion.transform.position, Quaternion.identity);
                        grenade.SetDamage(this.m_Champion.GetAttackDamage() * m_AttackDamageRatio);
                        grenade.OnThrow(point, m_Champion);
                        this.m_LastUseTime = Time.time;
                    }
                    else {
                        // Debug.Log("Out of range");
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