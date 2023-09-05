using UnityEngine;

public class KitegirlE2 : AAbility {
    [SerializeField] private KitegirlSmokescreen m_SmokescreenPrefab = null;
    [SerializeField] private float m_DashRange = 15f;

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
                    // TODO: Spawn smokescreen
                    this.m_LastUseTime = Time.time;
                    KitegirlSmokescreen kitegirlSmokescreen = Instantiate(m_SmokescreenPrefab,
                        m_Champion.transform.position, Quaternion.identity);
                    kitegirlSmokescreen.OnThrow(m_Champion);

                    m_Champion.Stop();
                    (m_Champion as Kitegirl)?.SmokeScreenPushBack(m_DashRange);
                }
                else {
                    // Debug.Log("Out of range");
                }
            }
        }
    }
}