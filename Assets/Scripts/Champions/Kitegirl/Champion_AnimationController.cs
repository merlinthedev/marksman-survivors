using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Champion_AnimationController : MonoBehaviour
{
    [SerializeField] private Champion m_ChampionScript;
    [SerializeField] private Animator m_Animator;

    private int m_Dir;

    private void Update() {
        int dir = (int)Mathf.Floor((m_ChampionScript.GetGlobalDirectionAngle() + 22.5f) / 45.0f) % 8;

    }

    public void Animate() {

    }
}
