using System.Runtime.CompilerServices;
using TMPro;
using TreeEditor;
using UnityEngine;

namespace _Scripts.Enemies {
    public class EnemyDamageNumberHelper : MonoBehaviour {
        [SerializeField] private TMP_Text m_DamageText;
        private bool shouldMove = false;
        private Vector3 initialPosition;

        private float currentDisplayedDamage;
        private float lastDisplayTime;

        public void Initialize() {
            initialPosition = transform.position;
        }

        private void Update() {
            if (Time.time > lastDisplayTime + 0.7f) {
                Reset();
            }

            if (shouldMove) {
                transform.position += Vector3.up * Time.deltaTime;
                // m_DamageText.alpha -= Time.deltaTime;
            }
        }

        public void SetDamage(float damage) {
            lastDisplayTime = Time.time;
            if (currentDisplayedDamage > 0) {
                if (Time.time < lastDisplayTime + 0.5f) {
                    currentDisplayedDamage += damage;
                }
            } else {
                currentDisplayedDamage = damage;
                shouldMove = true;
            }

            lastDisplayTime = Time.time;
            // Debug.Log("Setting damage to: " + currentDisplayedDamage + ".");
            m_DamageText.SetText(currentDisplayedDamage.ToString());
        }

        private void Reset() {
            m_DamageText.SetText("");
            currentDisplayedDamage = 0;
            transform.position = new Vector3(transform.position.x, initialPosition.y, transform.position.z);
            initialPosition = transform.position;
            shouldMove = false;
        }
    }
}