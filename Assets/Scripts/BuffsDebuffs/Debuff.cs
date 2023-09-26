using System.Collections;
using Entities;
using UnityEngine;

namespace BuffsDebuffs {
    public class Debuff {
        private IDamageable target;
        private IDamager source;
        private DebuffType debuffType;

        private float startTime = 0;
        private float duration;
        private float value;

        private float interval;
        private IEnumerator intervalCoroutine;

        public static Debuff CreateDebuff(IDamageable target, IDamager source, DebuffType debuffType, float duration,
            float value,
            float interval = -1) {
            return new Debuff(target, source, debuffType, duration, value, interval);
        }

        private Debuff(IDamageable target, IDamager source, DebuffType debuffType, float duration, float value,
            float interval = -1) {
            this.target = target;
            this.source = source;
            this.debuffType = debuffType;
            this.duration = duration;
            this.value = value;
            this.interval = interval;

            startTime = Time.time;

            if (interval > 0) StartCoroutines();
        }

        private void StartCoroutines() {
            intervalCoroutine = IntervalCoroutine();
            (target as MonoBehaviour)?.StartCoroutine(intervalCoroutine);
        }

        private IEnumerator IntervalCoroutine() {
            while (true) {
                yield return new WaitForSeconds(interval);
                // Debug.Log("Interval coroutine");
                // if the source is also an instance of IDamageable, apply damage

                // target.TakeFlatDamage(value);
                source.DealDamage(target, value);
            }
        }

        public void CheckForExpiration() {
            if (Time.time > startTime + duration) {
                (target as IDebuffable)?.RemoveDebuff(this);
                if (intervalCoroutine != null) {
                    (target as MonoBehaviour)?.StopCoroutine(intervalCoroutine);
                }
            }
        }

        public DebuffType GetDebuffType() {
            return debuffType;
        }

        public float GetDuration() {
            return duration;
        }

        public float GetValue() {
            return value;
        }


        public enum DebuffType {
            SLOW,
            BURN
        }
    }
}