using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Champion_AnimationController : MonoBehaviour {
    [SerializeField] private Champion m_ChampionScript;
    [SerializeField] private Animator m_Animator;
    private bool m_Moving;

    private int m_Dir;

    private void Update() {
        m_Dir = (int)Mathf.Floor((m_ChampionScript.GetGlobalDirectionAngle() + 135 + 22.5f) / 45.0f) % 8;
        Debug.Log(m_Dir);
        m_Animator.SetInteger("Dir", m_Dir);
        Move();
    }

    public void Move() {
        if (m_ChampionScript.IsMoving && !m_Moving) {
            m_Animator.SetTrigger("Move");
            m_Moving = true;
        }
        else if (!m_ChampionScript.IsMoving) {
            m_Moving = false;
            m_Animator.SetBool("IsMoving", false);
        }

    }

    public void Attack() {
        m_Animator.SetTrigger("Attack");
        m_Animator.SetInteger("Dir", m_Dir);
    }
}