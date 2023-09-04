using UnityEngine;

public class KitegirlE2 : AAbility {
    [SerializeField] private KitegirlSmokescreen m_SmokescreenPrefab = null;
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
                }
                else {
                    // Debug.Log("Out of range");
                }
            }
        }
    }
}