using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI {
    public class UIGameStatistics : MonoBehaviour {
        [SerializeField] private TMP_Text kills;
        [SerializeField] private TMP_Text gold;
        [SerializeField] private TMP_Text time;

        private void Update() {
            //m_Kills.text = "Kills: " + GameManager.Instance.GetKills();
            //m_Gold.text = "Gold: " + GameManager.Instance.GetGold();
            //m_Time.text = "Time: " + GameManager.Instance.GetTime();
        }
    }
}