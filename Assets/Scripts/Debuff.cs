using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class Debuff {
    private IDamageable m_Target;
    private DebuffType m_DebuffType;

    private float m_StartTime = 0;
    private float m_Duration;
    private float m_Value;

    private float m_Interval;
    private IEnumerator m_IntervalCoroutine;

    public static Debuff CreateDebuff(IDamageable target, DebuffType debuffType, float duration, float value,
        float interval = -1) {
        return new Debuff(target, debuffType, duration, value, interval);
    }

    private Debuff(IDamageable target, DebuffType debuffType, float duration, float value, float interval = -1) {
        m_Target = target;
        m_DebuffType = debuffType;
        m_Duration = duration;
        m_Value = value;
        m_Interval = interval;

        m_StartTime = Time.time;

        if (interval > 0) StartCoroutines();
    }

    private void StartCoroutines() {
        m_IntervalCoroutine = IntervalCoroutine();
        (m_Target as MonoBehaviour)?.StartCoroutine(m_IntervalCoroutine);
    }

    private IEnumerator IntervalCoroutine() {
        while (true) {
            yield return new WaitForSeconds(m_Interval);
            // Debug.Log("Interval coroutine");
            // if the source is also an instance of IDamageable, apply damage

            m_Target.TakeFlatDamage(m_Value);
        }
    }

    public void CheckForExpiration() {
        if (Time.time > m_StartTime + m_Duration) {
            (m_Target as IDebuffable)?.RemoveDebuff(this);
            if (m_IntervalCoroutine != null) {
                (m_Target as MonoBehaviour)?.StopCoroutine(m_IntervalCoroutine);
            }
        }
    }

    public DebuffType GetDebuffType() {
        return m_DebuffType;
    }

    public float GetDuration() {
        return m_Duration;
    }

    public float GetValue() {
        return m_Value;
    }


    public enum DebuffType {
        Slow,
        Burn
    }
}