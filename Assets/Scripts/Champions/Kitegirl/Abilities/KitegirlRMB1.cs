using Champions.Abilities;
using Champions.Kitegirl.Entities;
using UnityEngine;

namespace Champions.Kitegirl.Abilities {
    public class KitegirlRMB1 : Ability {
        [SerializeField] private KitegirlSlowArea m_KitegirlSlowAreaPrefab;

        [SerializeField] private float lifespan = 0.2f;
        [SerializeField] private float slowPercentage = 0.33f; // 0-1 Normalized
        [SerializeField] private float slowDuration = 3f; // Seconds
        [SerializeField] private float ADDamageRatio = 1.2f; // 0-1 Normalized


        public override void OnUse() {
            if (IsOnCooldown()) return;

            // Log("RMB1", Logger.Color.PINK, this);

            Vector3 mousePosition = Vector3.zero;

            int layer = LayerMask.GetMask("ExcludeFromRaycast");
            layer = ~layer;

            // get the cursor hover position in the world
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, float.MaxValue, layer)) {
                Vector3 point = hit.point;
                point.y = champion.transform.position.y;

                mousePosition = point;
            }

            // Debug.Log("<color=red>MousePosition: " + mousePosition + "</color>", this);

            if (mousePosition == Vector3.zero) {
                // Log("RMB1 break 1", Logger.Color.RED, this);
                return;
            }

            // get the global angle of the direction from the champion to the mouse position
            Vector3 mouseToChampionDirection = mousePosition - champion.transform.position;
            float mouseToChampionAngle = Vector3.SignedAngle(Vector3.forward, mouseToChampionDirection, Vector3.up);


            // Log("Angle: " + mouseToChampionAngle, Util.Logger.Color.YELLOW, this);

            //KitegirlSlowArea kitegirlSlowArea = Instantiate(m_KitegirlSlowAreaPrefab,
            //    champion.transform.position + mouseToChampionDirection.normalized *
            //    m_KitegirlSlowAreaPrefab.transform.localScale.x / 2,
            //    Quaternion.Euler(0, mouseToChampionAngle - 90, 0));

            KitegirlSlowArea kitegirlSlowArea = Instantiate(m_KitegirlSlowAreaPrefab,
                champion.transform.position + mouseToChampionDirection.normalized *
                m_KitegirlSlowAreaPrefab.transform.localScale.x,
                Quaternion.Euler(90, mouseToChampionAngle, 0));

            kitegirlSlowArea.OnThrow(champion as Kitegirl);
            kitegirlSlowArea.SetLifespan(lifespan);
            kitegirlSlowArea.SetSlowDuration(slowDuration);
            kitegirlSlowArea.SetSlowPercentage(slowPercentage);
            kitegirlSlowArea.SetADDamageRatio(ADDamageRatio);

            // Log("Done, Calling base.OnUse()", Logger.Color.PINK, this);

            base.OnUse();
        }
    }
}