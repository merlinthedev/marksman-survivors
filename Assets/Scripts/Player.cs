using Events;
using System;
using UnityEngine;
using UnityEngine.UI;
using Util;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour {
    private Collider m_Collider;
    private Rigidbody m_Rigidboby;

    private Vector3 m_HitPoint;

    [Header("Stats")]
    [SerializeField] private float m_MaxHealth = 1000f;

    [SerializeField] private float m_CurrentHealth = 1000f;
    [SerializeField] private float m_Damage = 90f;
    [SerializeField] private float m_AttackSpeed = 1f;

    [SerializeField] private Bullet m_BulletPrefab;

    [SerializeField] private float m_MovementSpeed = 5f;
    private float m_GlobalMovementDirectionAngle = 0f;
    private Vector3 m_LastKnownDirection = Vector3.left;

    [SerializeField] private Texture2D m_CursorTexture, m_AttackCursorTexture;

    private bool m_CanMove = true;

    [SerializeField] private LayerMask m_AttackLayerMask;

    [Header("UI")]
    [SerializeField] private Image m_HealthBar;

    private float m_InitialHealthBarWidth;
    [SerializeField] private Image m_AttackBar;

    private float m_InitialAttackBarWidth;
    private float m_LastAttackTime = 0f;
    private bool CanAttack => Time.time > m_LastAttackTime + (1f / m_AttackSpeed);
    private bool m_ShouldUpdateAttackBarOnceMore = false;

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
        m_InitialAttackBarWidth = m_AttackBar.rectTransform.sizeDelta.x;
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
                    m_HitPoint = point;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && CanAttack) {
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
                    m_HitPoint = transform.position; // ???
                    m_LastAttackTime = Time.time;

                    Invoke(nameof(EnableMovement), .1f);
                    m_Rigidboby.velocity = Vector3.zero;

                    // shoot 3 bullets in burst mode

                    ShootBullet_Recursive(true, point);

                    /*
                     *      Bullet bullet = Instantiate(m_BulletPrefab, transform.position, Quaternion.identity);
                     *      bullet.SetTarget(point + randomBulletSpread);
                     *      bullet.SetDamage(m_Damage);
                     * 
                     */
                }
            }
        }

        if (m_HitPoint != Vector3.zero && m_CanMove) {
            Move();
        }

        if (!CanAttack) {
            UpdateAttackBar();
            m_ShouldUpdateAttackBarOnceMore = true;
        }
        else {
            if (m_ShouldUpdateAttackBarOnceMore) {
                // update the attack bar to be full
                m_AttackBar.rectTransform.sizeDelta = new Vector2(
                    m_InitialAttackBarWidth,
                    m_AttackBar.rectTransform.sizeDelta.y
                );
                m_ShouldUpdateAttackBarOnceMore = false;
            }
        }
    }


    private void EnableMovement() {
        m_CanMove = true;
    }


    private void Move() {
        Vector3 direction = m_HitPoint - transform.position;

        m_GlobalMovementDirectionAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        // Debug.Log("Global Direction Angle: " + globalDirectionAngle);
        // DrawDirectionRays();

        if (direction.magnitude < 0.1f) {
            m_HitPoint = Vector3.zero;
            m_Rigidboby.velocity = Vector3.zero;
            return;
        }

        m_Rigidboby.velocity = direction.normalized * m_MovementSpeed;
        m_LastKnownDirection = direction.normalized;
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

    private void UpdateAttackBar() {
        float attackPercentage = (Time.time - m_LastAttackTime) / (1f / m_AttackSpeed);

        m_AttackBar.rectTransform.sizeDelta = new Vector2(
            attackPercentage * m_InitialAttackBarWidth,
            m_AttackBar.rectTransform.sizeDelta.y
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

    public Vector3 GetCurrentMovementDirection() {
        return m_Rigidboby.velocity.normalized == Vector3.zero ? m_LastKnownDirection : m_Rigidboby.velocity.normalized;
    }

    public float GetGlobalDirectionAngle() {
        return m_GlobalMovementDirectionAngle;
    }


    private void DrawDirectionRays() {
        // Direction ray
        Debug.DrawRay(transform.position, GetCurrentMovementDirection() * 10f, Color.red);

        // Right ray
        Vector3 rightDirection = Quaternion.Euler(0, 45, 0) * GetCurrentMovementDirection();
        // Debug.DrawRay(transform.position, rightDirection * 10f, Color.green);
        // Draw the ray until the edge of the viewport
        Debug.DrawRay(transform.position, rightDirection * 10f, Color.green, 0);

        // Left ray
        Vector3 leftDirection = Quaternion.Euler(0, -45, 0) * GetCurrentMovementDirection();
        Debug.DrawRay(transform.position, leftDirection * 10f, Color.blue, 0);
    }

    private int recurseCount = 0;
    private int maxRecurseCount = 3;

    private void ShootBullet_Recursive(bool shouldCallRecursive, Vector3 target) {
        recurseCount++;

        Vector3 randomBulletSpread = new Vector3(
            UnityEngine.Random.Range(-0.1f, 0.1f),
            0,
            UnityEngine.Random.Range(-0.1f, 0.1f)
        );

        Bullet bullet = Instantiate(m_BulletPrefab, transform.position, Quaternion.identity);
        bullet.SetTarget(target + randomBulletSpread);
        bullet.SetDamage(m_Damage);

        if (shouldCallRecursive && recurseCount < maxRecurseCount) {
            Utilities.InvokeDelayed(() => { ShootBullet_Recursive(true, target); }, 0.05f, this);
        }
        else {
            recurseCount = 0;
        }
    }
}