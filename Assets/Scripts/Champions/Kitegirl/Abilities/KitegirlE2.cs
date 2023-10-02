using Champions.Abilities;
using Champions.Kitegirl.Entities;
using UnityEngine;

namespace Champions.Kitegirl.Abilities {
    public class KitegirlE2 : AAbility {
        [SerializeField] private KitegirlSmokescreen smokescreenPrefab = null;
        [SerializeField] private float dashRange = 3f;
        [SerializeField] private float yForceOffset = 4f;

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


            // TODO: Spawn smokescreen
            KitegirlSmokescreen kitegirlSmokescreen = Instantiate(smokescreenPrefab,
                new Vector3(champion.transform.position.x, 0f, champion.transform.position.z), Quaternion.Euler(90, 0 ,0));
            kitegirlSmokescreen.OnThrow(champion as Kitegirl);

            champion.Stop();
            (champion as Kitegirl)?.SmokeScreenPushBack(dashRange, yForceOffset,
                mousePosition);

            base.OnUse();
        }
    }
}