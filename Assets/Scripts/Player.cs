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
    [SerializeField] private Image m_AttackBar;
    [SerializeField] private Image m_QBar;
    private bool QIsOnCooldown;
    [SerializeField] private Image m_WBar;
    [SerializeField] private Image m_EBar;
    [SerializeField] private Image m_RBar;
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
            m_SelectedChampion.OnAbility(KeyCode.Q);
            UpdateQCooldown();
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            m_SelectedChampion.OnAbility(KeyCode.W);
            UpdateWCooldown();
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            m_SelectedChampion.OnAbility(KeyCode.E);
            UpdateECooldown();
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            m_SelectedChampion.OnAbility(KeyCode.R);
            UpdateRCooldown();
        }
    }

    private void Update() {
        HandleMoveClick();
        HandleAttackClick();
        HandleAbilityClicks();
        HandleAbilityCooldowns();
        if (!m_SelectedChampion.CanAttack) {
            UpdateAttackBar();
        }
    }

    public void TakeDamage(float damage) {
        m_SelectedChampion.TakeDamage(damage);
        UpdateHealthBar();
    }

    private void UpdateHealthBar() {
        float healthPercentage = m_SelectedChampion.GetCurrentHealth() / m_SelectedChampion.GetMaxHealth();

        m_HealthBar.fillAmount = healthPercentage;
    }

    private void UpdateAttackBar() {
        float attackPercentage = (Time.time - m_SelectedChampion.GetLastAttackTime()) / (1f / m_SelectedChampion.GetAttackSpeed());

        m_AttackBar.fillAmount = 1 - attackPercentage;
    }

    private void HandleAbilityCooldowns() {
        if(m_SelectedChampion.GetAbilities()[0].IsOnCooldown()) {
            UpdateQCooldown();
        }
        if (m_SelectedChampion.GetAbilities()[1].IsOnCooldown()) {
            UpdateWCooldown();
        }
        if (m_SelectedChampion.GetAbilities()[2].IsOnCooldown()) {
            UpdateECooldown();
        }
        if (m_SelectedChampion.GetAbilities()[3].IsOnCooldown()) {
            UpdateRCooldown();
        }
    }
    private void UpdateQCooldown() {
        float QCoolDownPercentage = m_SelectedChampion.GetAbilities()[0].GetCurrentCooldown() / m_SelectedChampion.GetAbilities()[0].GetAbilityCooldown();
        m_QBar.fillAmount = 1 - QCoolDownPercentage;
    }

    private void UpdateWCooldown() {
        float WCoolDownPercentage = m_SelectedChampion.GetAbilities()[1].GetCurrentCooldown() / m_SelectedChampion.GetAbilities()[1].GetAbilityCooldown();
        m_WBar.fillAmount = 1 - WCoolDownPercentage;
    }

    private void UpdateECooldown() {
        float ECoolDownPercentage = m_SelectedChampion.GetAbilities()[2].GetCurrentCooldown() / m_SelectedChampion.GetAbilities()[2].GetAbilityCooldown();
        m_EBar.fillAmount = 1 - ECoolDownPercentage;
    }

    private void UpdateRCooldown() {
        float RCoolDownPercentage = m_SelectedChampion.GetAbilities()[3].GetCurrentCooldown() / m_SelectedChampion.GetAbilities()[3].GetAbilityCooldown();
        m_RBar.fillAmount = 1 - RCoolDownPercentage;
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