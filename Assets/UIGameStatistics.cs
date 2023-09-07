using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIGameStatistics : MonoBehaviour
{
    [SerializeField] private TMP_Text m_Kills;
    [SerializeField] private TMP_Text m_Gold;
    [SerializeField] private TMP_Text m_Time;

    private void Update() {
        //m_Kills.text = "Kills: " + GameManager.Instance.GetKills();
        //m_Gold.text = "Gold: " + GameManager.Instance.GetGold();
        //m_Time.text = "Time: " + GameManager.Instance.GetTime();
    }

}
