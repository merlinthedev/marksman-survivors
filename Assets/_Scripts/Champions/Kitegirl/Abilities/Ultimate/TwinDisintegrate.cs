using _Scripts.Champions.Abilities;
using _Scripts.Champions.Kitegirl.Entities;
using _Scripts.Util;
using UnityEditor.Rendering;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using Logger = _Scripts.Util.Logger;

namespace _Scripts.Champions.Kitegirl.Abilities.Ultimate {
    public class TwinDisintegrate : Ability {
        [SerializeField] private GameObject twinDisintegratePrefab;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float totalRotation = 360f;

        [SerializeField] [Tooltip("Percentage of the champions AD, 2 = 200%")]
        private float damagePercentage = 2f;

        public override void OnUse() {
            if (IsOnCooldown()) return;

            var direction = Utilities.GetPointToMouseDirection(champion.transform.position);
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            for (int i = 0; i < 2; i++) {
                var twinDisintegrate =
                    Instantiate(twinDisintegratePrefab, champion.transform.position, Quaternion.Euler(0, angle, 0));
                var controller = twinDisintegrate.GetComponentInChildren<TwinDisintegrateController>();
                controller.Init(champion, champion.GetAttackDamage() * damagePercentage,
                    i == 0 ? rotationSpeed : -rotationSpeed, totalRotation);
                twinDisintegrate.transform.parent = champion.transform;
            }

            base.OnUse();
        }
    }
}