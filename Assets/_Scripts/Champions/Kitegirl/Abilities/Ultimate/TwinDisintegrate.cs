using _Scripts.Champions.Abilities;
using _Scripts.Champions.Kitegirl.Entities;
using _Scripts.Entities;
using _Scripts.Util;
using UnityEngine;

namespace _Scripts.Champions.Kitegirl.Abilities.Ultimate {
    public class TwinDisintegrate : Ability {
        [SerializeField] private KitegirlGrenade stickyBombPrefab;

        public override void Hook(Champion champion) {
            base.Hook(champion);
        }

        public override void OnUse() {
            if (IsOnCooldown()) return;

            int steps = 16;
            float radius = 10f;
            float x;
            float y = 1.2f;
            float z;

            float angle = 20f;

            var championPos = this.champion.transform.position;

            Utilities.DelayedForLoop(steps + 1, 0.05f, () => {
                x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
                z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

                var position = new Vector3(x, y, z) + championPos;
                var sticky = Instantiate(stickyBombPrefab, position, Quaternion.identity);
                sticky.SetDamage(this.champion.GetAttackDamage() * 2f);
                sticky.OnThrow(position, this.champion);
                sticky.SetDetonateTime(0.8f);
                sticky.SetDamageRadius(6f);

                angle += 360f / steps;
            }, this.champion);

            base.OnUse();

        }


    }
}