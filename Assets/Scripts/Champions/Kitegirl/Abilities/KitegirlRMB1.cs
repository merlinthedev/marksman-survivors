using Champions.Abilities;
using Champions.Kitegirl.Entities;
using UnityEngine;
using static Util.Logger;

namespace Champions.Kitegirl.Abilities {
    public class KitegirlRMB1 : AAbility {
        [SerializeField] private KitegirlSlowArea m_KitegirlSlowAreaPrefab;

        public override void OnUse() {
            if (IsOnCooldown()) return;

            Vector3 mousePosition = Vector3.zero;

            // get the cursor hover position in the world
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                Vector3 point = hit.point;
                point.y = this.champion.transform.position.y;

                mousePosition = point;
            }

            // Debug.Log("<color=red>MousePosition: " + mousePosition + "</color>", this);

            if (mousePosition == Vector3.zero) return;

            // get the global angle of the direction from the champion to the mouse position
            Vector3 mouseToChampionDirection = mousePosition - champion.transform.position;
            float mouseToChampionAngle = Vector3.SignedAngle(Vector3.forward, mouseToChampionDirection, Vector3.up);


            Log("Angle: " + mouseToChampionAngle, Util.Logger.Color.YELLOW, this);

            KitegirlSlowArea kitegirlSlowArea = Instantiate(m_KitegirlSlowAreaPrefab,
                champion.transform.position + mouseToChampionDirection.normalized *
                m_KitegirlSlowAreaPrefab.transform.localScale.x / 2,
                Quaternion.Euler(0, mouseToChampionAngle - 90, 0));

            kitegirlSlowArea.OnThrow(champion as Kitegirl);
            base.OnUse();
        }
    }
}