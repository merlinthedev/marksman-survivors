using System.Collections;
using TMPro;
using UnityEngine;

namespace UI {
    public class TimeHandler : MonoBehaviour {
        [SerializeField] private TMP_Text m_TimeText;

        private int seconds;
        private int minutes;

        private void Start() {
            StartCoroutine(Clock());
        }

        private IEnumerator Clock() {
            while (true) {
                // TODO: replace later with some type of gamerunning boolean
                seconds++;
                if (seconds == 60) {
                    minutes++;
                    seconds = 0;
                }

                m_TimeText.text = $"{minutes:00}:{seconds:00}";

                yield return new WaitForSeconds(1f);
            }
        }
    }
}