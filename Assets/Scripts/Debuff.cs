using System.Collections;
using System.Collections.Generic;
using UnityEditor.TextCore.Text;
using UnityEngine;

public class Debuff {
    private IDebuffer source;
    private DebuffType debuffType;

    private float duration;
    private float value;

    public static Debuff CreateDebuff(IDebuffer source, DebuffType debuffType, float duration, float value) {
        return new Debuff(source, debuffType, duration, value);
    }

    private Debuff(IDebuffer source, DebuffType debuffType, float duration, float value) {
        this.source = source;
        this.debuffType = debuffType;
        this.duration = duration;
        this.value = value;
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
        SLOW
    }
}