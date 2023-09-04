using UnityEngine;

public class KitegirlW : AAbility {
    [SerializeField] private KitegirlGrenade m_GrenadePrefab;
    [SerializeField] private float m_AbilityRange = 10f;

    private float m_AttackDamageRatio = 0.6f; // 0.6f => 60% of AD

    public override void OnUse() {
        if (IsOnCooldown()) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) {
            if (hit.collider.gameObject.CompareTag("Ground")) {
                Vector3 point = hit.point;
                point.y = this.m_Champion.transform.position.y - 0.2f;

                // Debug.Log("Point: " + point, this);

                if (DistanceCheck(point)) {
                    KitegirlGrenade grenade =
                        Instantiate(m_GrenadePrefab, this.m_Champion.transform.position, Quaternion.identity);
                    grenade.SetTargetPoint(point);
                    grenade.SetSourceEntity(this.m_Champion);
                    grenade.SetDamage(this.m_Champion.GetChampionStatistics().AttackDamage * m_AttackDamageRatio);
                    grenade.OnThrow();
                    this.m_LastUseTime = Time.time;
                }
                else {
                    // Debug.Log("Out of range");
                }
            }
        }
    }

    private bool DistanceCheck(Vector3 point) {
        return (this.m_Champion.transform.position - point).magnitude <= m_AbilityRange;
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