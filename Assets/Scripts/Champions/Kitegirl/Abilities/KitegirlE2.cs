using Champions.Kitegirl.Entities;
using Events;
using UnityEngine;

namespace Champions.Kitegirl.Abilities {
    public class KitegirlE2 : AAbility {
        [SerializeField] private KitegirlSmokescreen m_SmokescreenPrefab = null;
        [SerializeField] private float m_DashRange = 3f;
        [SerializeField] private float m_YForceOffset = 4f;

        public override void OnUse() {
            if (IsOnCooldown()) return;
            base.OnUse();

            Vector3 mousePosition = Vector3.zero;

            // get the cursor hover position in the world
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                Vector3 point = hit.point;
                point.y = this.m_Champion.transform.position.y;

                mousePosition = point;
            }

            Debug.Log("<color=red>MousePosition: " + mousePosition + "</color>", this);

            if (mousePosition == Vector3.zero) return;


            // TODO: Spawn smokescreen
            m_LastUseTime = Time.time;
            KitegirlSmokescreen kitegirlSmokescreen =
                Instantiate(m_SmokescreenPrefab, new Vector3(m_Champion.transform.position.x, 0.5f, m_Champion.transform.position.z), Quaternion.identity);
            kitegirlSmokescreen.OnThrow(m_Champion);

            m_Champion.Stop();
            (m_Champion as global::Champions.Kitegirl.Kitegirl)?.SmokeScreenPushBack(m_DashRange, m_YForceOffset,
                mousePosition);
        }
    }
}