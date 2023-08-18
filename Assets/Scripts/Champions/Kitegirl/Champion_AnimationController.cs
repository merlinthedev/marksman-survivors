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
        m_Animator.SetInteger("direction", m_Dir);
        Debug.Log("GlobalDirectionAngle: " + m_ChampionScript.GetGlobalDirectionAngle() + ", Dir: " + m_Dir, this);
        Animate();
    }

    private void Animate() {
        if (m_ChampionScript.IsMoving) {
            m_Animator.SetBool("isRunning", true);
        } else {
            m_Animator.SetBool("isRunning", false);
        }
    }

    public void Attack() {
        return;
    }
}