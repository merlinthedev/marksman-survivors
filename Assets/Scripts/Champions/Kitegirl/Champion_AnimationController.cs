using UnityEngine;

namespace Champions.Kitegirl {
    public class Champion_AnimationController : MonoBehaviour {
        [SerializeField] private Champion m_ChampionScript;
        [SerializeField] private Animator m_Animator;
        private bool m_Moving;
        private int m_CurrentDir;

        private int m_Dir;

        private void Update() {
            int globalDirection = Mathf.FloorToInt((m_ChampionScript.GetGlobalDirectionAngle() + 22.5f) / 45.0f);
            m_Dir = (globalDirection + 7) % 8;
            m_Animator.SetInteger("Dir", m_Dir);
            Move();
        }

        private void Move() {
            if (m_ChampionScript.IsMoving && !m_Moving || m_ChampionScript.IsMoving && m_CurrentDir != m_Dir) {
                m_CurrentDir = m_Dir;
                m_Moving = true;
                m_Animator.SetTrigger("Move");
                m_Animator.SetBool("IsMoving", true);
            } else if (!m_ChampionScript.IsMoving) {
                m_Moving = false;
                m_Animator.SetBool("IsMoving", false);
            }
            else {
                return;
            }

        }

        public void Attack() {
            m_Animator.SetTrigger("Attack");
            m_Animator.SetInteger("Dir", m_Dir);
        }
    }
}