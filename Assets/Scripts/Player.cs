using Events;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {
    private Collider m_Collider;
    private Rigidbody m_Rigidboby;

    private Vector3 hitPoint;

    [SerializeField] private float m_MaxHealth = 1000f;
    [SerializeField] private float m_CurrentHealth = 1000f;
    [SerializeField] private float m_Damage = 90f;

    [SerializeField] private Bullet m_BulletPrefab;
    [SerializeField] private Image m_HealthBar;
    private float m_InitialHealthBarWidth;
    [SerializeField] private float m_MovementSpeed = 5f;
    [SerializeField] private Texture2D m_CursorTexture, m_AttackCursorTexture;
    private bool m_CanMove = true;
    [SerializeField] private LayerMask m_AttackLayerMask;

    private void OnEnable() {
        EventBus<EnemyStartHoverEvent>.Subscribe(OnEnemyStartHover);
        EventBus<EnemyStopHoverEvent>.Subscribe(OnEnemyStopHover);
    }

    private void OnDisable() {
        EventBus<EnemyStartHoverEvent>.Unsubscribe(OnEnemyStartHover);
        EventBus<EnemyStopHoverEvent>.Unsubscribe(OnEnemyStopHover);
    }

    private void Start() {
        m_Collider = GetComponent<Collider>();
        m_Rigidboby = GetComponent<Rigidbody>();

        if (m_Collider == null || m_Rigidboby == null) {
            throw new Exception("Missing collider or rigidbody");
        }

        m_CurrentHealth = m_MaxHealth;
        m_InitialHealthBarWidth = m_HealthBar.rectTransform.sizeDelta.x;
        UpdateHealthBar();

        SetDefaultCursorTexture();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.gameObject.CompareTag("Ground")) {
                    var point = hit.point;
                    point.y = transform.position.y;
                    hitPoint = point;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.gameObject.CompareTag("Ground") || hit.collider.gameObject.CompareTag("Enemy")) {
                    var point = hit.point;
                    point.y = transform.position.y;

                    if (!m_CanMove) {
                        return;
                    }

                    m_CanMove = false;
                    m_Rigidboby.velocity = Vector3.zero;
                    Invoke(nameof(EnableMovement), .01f);

                    Bullet bullet = Instantiate(m_BulletPrefab, transform.position, Quaternion.identity);
                    bullet.SetTarget(point);
                    bullet.SetDamage(m_Damage);
                }
            }
        }

        if (hitPoint != Vector3.zero && m_CanMove) {
            Move();
        }
    }

    private void EnableMovement() {
        m_CanMove = true;
    }

    private void Move() {
        Vector3 direction = hitPoint - transform.position;

        if (direction.magnitude < 0.1f) {
            hitPoint = Vector3.zero;
            m_Rigidboby.velocity = Vector3.zero;
            return;
        }

        m_Rigidboby.velocity = direction.normalized * m_MovementSpeed;
    }

    public void TakeDamage(float damage) {
        m_CurrentHealth -= damage;
        UpdateHealthBar();

        if (m_CurrentHealth <= 0) {
            Destroy(gameObject);
        }
    }

    private void UpdateHealthBar() {
        float healthPercentage = m_CurrentHealth / m_MaxHealth;

        m_HealthBar.rectTransform.sizeDelta = new Vector2(
            healthPercentage * m_InitialHealthBarWidth,
            m_HealthBar.rectTransform.sizeDelta.y
        );
    }

    private void SetDefaultCursorTexture() {
        Cursor.SetCursor(m_CursorTexture, Vector2.zero, CursorMode.Auto);
    }

    private void OnEnemyStartHover(EnemyStartHoverEvent e) {
        Cursor.SetCursor(m_AttackCursorTexture, Vector2.zero, CursorMode.Auto);
    }

    private void OnEnemyStopHover(EnemyStopHoverEvent e) {
        SetDefaultCursorTexture();
    }
}