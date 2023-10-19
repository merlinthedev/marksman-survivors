using System;
using UnityEngine;

namespace Champions.Kitegirl {
    public class Champion_AnimationController : MonoBehaviour {
        [SerializeField] private Champion m_ChampionScript;
        [SerializeField] private Animator m_Animator;
        private bool isMoving;
        private int currentDir;

        private int dir;

        private void Update() {
            int globalDirection = Mathf.FloorToInt((m_ChampionScript.GetGlobalDirectionAngle() + 22.5f) / 45.0f);
            dir = (globalDirection + 7) % 8;
            m_Animator.SetInteger("Dir", dir);
            Debug.Log("Dir: " + dir);
            Move();
        }

        private void Move() {
            if (m_ChampionScript.IsMoving && !isMoving || m_ChampionScript.IsMoving && currentDir != dir) {
                currentDir = dir;
                isMoving = true;
                m_Animator.SetTrigger("Move");
                m_Animator.SetBool("IsMoving", true);
            } else if (!m_ChampionScript.IsMoving) {
                isMoving = false;
                m_Animator.SetBool("IsMoving", false);
            } else {
                return;
            }
        }

        public void Attack() {
            m_Animator.SetTrigger("Attack");
            m_Animator.SetInteger("Dir", dir);
        }

        public void SetDirection(float angle) {
            Debug.Log("Setting angle");
            int globalDirection = Mathf.FloorToInt((angle + 22.5f) / 45.0f);
            dir = (globalDirection + 7) % 8;
            m_Animator.SetInteger("Dir", dir);
        }
    }
}