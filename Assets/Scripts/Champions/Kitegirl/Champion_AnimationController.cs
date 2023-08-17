using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Champion_AnimationController : MonoBehaviour {
    [SerializeField] private Champion m_ChampionScript;
    [SerializeField] private Animator m_Animator;

    private int m_Dir;

    private void Update() {
        m_Dir = (int)Mathf.Floor((m_ChampionScript.GetGlobalDirectionAngle() + 22.5f) / 45.0f) % 8;
        m_Dir--;
        if (m_Dir == -1) m_Dir = 7;
        Debug.Log("GlobalDirectionAngle: " + m_ChampionScript.GetGlobalDirectionAngle() + ", Dir: " + m_Dir, this);
    }

    public void Animate() {

    }

    public void Attack() {
        return;
    }
}