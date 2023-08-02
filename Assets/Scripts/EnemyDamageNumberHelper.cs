using TMPro;
using UnityEngine;

public class EnemyDamageNumberHelper : MonoBehaviour {
    [SerializeField] private TMP_Text m_DamageText;
    private bool m_ShouldMove = false;

    public void Initialize(string damage) {
        m_DamageText.SetText(damage);
        m_ShouldMove = true;
        
        Invoke(nameof(Terminate), 0.6f);
    }

    private void Update() {
        if (m_ShouldMove) {
            transform.position += Vector3.up * Time.deltaTime;
        }
    }

    public void Terminate() {
        m_DamageText.SetText("");
        Destroy(gameObject);
    }
}