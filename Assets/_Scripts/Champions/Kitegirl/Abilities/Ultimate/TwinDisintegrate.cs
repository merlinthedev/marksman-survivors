using _Scripts.Champions.Abilities;
using _Scripts.Champions.Kitegirl.Entities;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

namespace _Scripts.Champions.Kitegirl.Abilities.Ultimate {
    public class TwinDisintegrate : Ability {
        [SerializeField] private GameObject twinDisintegratePrefab;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float totalRotation = 360f;

        [SerializeField] [Tooltip("Percentage of the champions AD, 2 = 200%")]
        private float damagePercentage = 2f;

        public override void OnUse() {
            if (IsOnCooldown()) return;

            var twinDisintegrate =
                Instantiate(twinDisintegratePrefab, champion.transform.position, Quaternion.identity);

            var twinDisintegrateController = twinDisintegrate.GetComponentInChildren<TwinDisintegrateController>();
            twinDisintegrateController.Init(champion, champion.GetAttackDamage() * 2, rotationSpeed, totalRotation);

            // instantiate another laser but rotate it counter-clockwise
            var twinDisintegrate2 =
                Instantiate(twinDisintegratePrefab, champion.transform.position, Quaternion.identity);

            var twinDisintegrateController2 = twinDisintegrate2.GetComponentInChildren<TwinDisintegrateController>();
            twinDisintegrateController2.Init(champion, champion.GetAttackDamage() * 2, -rotationSpeed, totalRotation);

            base.OnUse();
        }
    }
}