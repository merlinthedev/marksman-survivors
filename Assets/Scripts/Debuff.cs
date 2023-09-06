using System.Collections;
using UnityEngine;

public class Debuff {
    private IDebuffer source;
    private IDebuffable target;
    private DebuffType debuffType;

    private float startTime = 0;
    private float duration;
    private float value;

    private float interval;
    private IEnumerator intervalCoroutine;

    public static Debuff CreateDebuff(IDebuffer source, DebuffType debuffType, float duration, float value,
        float interval = -1) {
        return new Debuff(source, debuffType, duration, value);
    }

    private Debuff(IDebuffer source, DebuffType debuffType, float duration, float value, float interval = -1) {
        this.source = source;
        this.debuffType = debuffType;
        this.duration = duration;
        this.value = value;
        this.interval = interval;

        startTime = Time.time;
        if (interval > 0) {
            intervalCoroutine = IntervalCoroutine();
            (source as MonoBehaviour)?.StartCoroutine(intervalCoroutine);
        }
    }

    private IEnumerator IntervalCoroutine() {
        while (true) {
            // if the source is also an instance of IDamageable, apply damage
            if (source is IDamageable) {
                (source as IDamageable)?.TakeFlatDamage(value);
            }
        }
    }

    public void CheckForExpiration() {
        if (Time.time > startTime + duration) {
            target.RemoveDebuff(this);
            source.AffectedEntities.Remove(target);
            if (intervalCoroutine != null) {
                (source as MonoBehaviour)?.StopCoroutine(intervalCoroutine);
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