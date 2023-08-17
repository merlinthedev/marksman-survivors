using Events;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

public class Player : MonoBehaviour {


    [Header("Stats")]
    [SerializeField] private Texture2D m_CursorTexture, m_AttackCursorTexture;

    [SerializeField] private LayerMask m_AttackLayerMask;

    [Header("UI")]
    [SerializeField] private Image m_HealthBar;

    private float m_InitialHealthBarWidth;
    [SerializeField] private Image m_AttackBar;
    private float m_InitialAttackBarWidth;
    private bool m_ShouldUpdateAttackBarOnceMore = false;
    [SerializeField] private Champion m_SelectedChampion;

    private void OnEnable() {
        EventBus<EnemyStartHoverEvent>.Subscribe(OnEnemyStartHover);
        EventBus<EnemyStopHoverEvent>.Subscribe(OnEnemyStopHover);
    }

    private void OnDisable() {
        EventBus<EnemyStartHoverEvent>.Unsubscribe(OnEnemyStartHover);
        EventBus<EnemyStopHoverEvent>.Unsubscribe(OnEnemyStopHover);
    }

    private void Start() {
        m_InitialHealthBarWidth = m_HealthBar.rectTransform.sizeDelta.x;
        m_InitialAttackBarWidth = m_AttackBar.rectTransform.sizeDelta.x;
        UpdateHealthBar();

        SetDefaultCursorTexture();
    }

    private bool m_FirstMove = true;

    private void HandleMoveClick() {
        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.gameObject.CompareTag("Ground")) {
                    var point = hit.point;
                    point.y = transform.position.y;
                    // m_HitPoint = point;

                    if (m_FirstMove) {
                        m_FirstMove = false;
                        EnemyManager.GetInstance().SetShouldSpawn(true);
                        // Debug.Log("Start enemy spawning");
                    }


                    m_SelectedChampion.SetMouseHitPoint(point);
                }
            }
        }
    }

    private void HandleAttackClick() {
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.A)) {
            m_SelectedChampion.OnAutoAttack();
        }
    }

    private void HandleAbilityClicks() {
        // If Q, W, E or R is pressed, call the m_SelectedChampion.OnAbility() method and pass in the correct KeyCode

        if (Input.GetKeyDown(KeyCode.Q)) {
            Debug.Log("Q Pressed");
            m_SelectedChampion.OnAbility(KeyCode.Q);
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            m_SelectedChampion.OnAbility(KeyCode.W);
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            m_SelectedChampion.OnAbility(KeyCode.E);
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            m_SelectedChampion.OnAbility(KeyCode.R);
        }
    }

    private void Update() {
        HandleMoveClick();
        HandleAttackClick();
        HandleAbilityClicks();

        if (!m_SelectedChampion.CanAttack) {
            UpdateAttackBar();
            m_ShouldUpdateAttackBarOnceMore = true;
        } else {
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

    public void TakeDamage(float damage) {
        m_SelectedChampion.TakeDamage(damage);
        UpdateHealthBar();
    }

    private void UpdateHealthBar() {
        float healthPercentage = m_SelectedChampion.GetCurrentHealth() / m_SelectedChampion.GetMaxHealth();

        m_HealthBar.rectTransform.sizeDelta = new Vector2(
            healthPercentage * m_InitialHealthBarWidth,
            m_HealthBar.rectTransform.sizeDelta.y
        );
    }

    private void UpdateAttackBar() {
        float attackPercentage = (Time.time - m_SelectedChampion.GetLastAttackTime()) / (1f / m_SelectedChampion.GetAttackSpeed());

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

    public Champion GetCurrentlySelectedChampion() {
        return m_SelectedChampion;
    }
}