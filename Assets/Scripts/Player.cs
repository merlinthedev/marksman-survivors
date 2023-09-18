using Champions;
using Enemy;
using EventBus;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;
using static Util.Logger;
using Logger = Util.Logger;

public class Player : MonoBehaviour {
    [Header("Stats")]
    [SerializeField] private Texture2D m_CursorTexture, m_AttackCursorTexture;

    [SerializeField] private LayerMask m_AttackLayerMask;

    [Header("UI")]
    [SerializeField] private Image m_HealthBar;

    [SerializeField] private Champion m_SelectedChampion;

    [Header("Other")]
    [SerializeField] private GameObject m_ClickAnimPrefab;

    private bool isPaused = false;

    private void OnEnable() {
        EventBus<EnemyStartHoverEvent>.Subscribe(OnEnemyStartHover);
        EventBus<EnemyStopHoverEvent>.Subscribe(OnEnemyStopHover);
        EventBus<ChampionHealthRegenerated>.Subscribe(OnChampionHealthRegenerated);
        EventBus<ChampionDamageTakenEvent>.Subscribe(OnChampionDamageTakenEvent);
        EventBus<ChampionLevelUpEvent>.Subscribe(OnChampionLevelUp);
        EventBus<ChampionAbilityChosenEvent>.Subscribe(OnChampionAbilityChosen);
    }

    private void OnDisable() {
        EventBus<EnemyStartHoverEvent>.Unsubscribe(OnEnemyStartHover);
        EventBus<EnemyStopHoverEvent>.Unsubscribe(OnEnemyStopHover);
        EventBus<ChampionHealthRegenerated>.Unsubscribe(OnChampionHealthRegenerated);
        EventBus<ChampionDamageTakenEvent>.Unsubscribe(OnChampionDamageTakenEvent);
        EventBus<ChampionLevelUpEvent>.Unsubscribe(OnChampionLevelUp);
        EventBus<ChampionAbilityChosenEvent>.Unsubscribe(OnChampionAbilityChosen);
    }

    private void Start() {
        SetDefaultCursorTexture();
    }

    private bool m_FirstMove = true;

    private bool hasClickedThisFrame = false;

    private void HandleMouseClicks() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            int layerMask = LayerMask.GetMask("ExcludeFromMovementClicks");
            layerMask = ~layerMask;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
                if (hit.collider.gameObject.CompareTag("Ground")) {
                    hasClickedThisFrame = true;
                    var point = hit.point;
                    Instantiate(m_ClickAnimPrefab, new Vector3(point.x, 0.2f, point.z), Quaternion.identity);
                    point.y = transform.position.y;

                    if (m_FirstMove) {
                        EnemyManager.GetInstance().SetShouldSpawn(true);
                        m_FirstMove = false;
                    }

                    m_SelectedChampion.SetMouseHitPoint(point);
                }

                if (hit.collider.gameObject.CompareTag("Enemy") ||
                    hit.collider.gameObject.CompareTag("KitegirlGrenade")) {
                    m_SelectedChampion.OnAutoAttack(hit.collider);
                }
            }
        }

        if (Input.GetKey(KeyCode.Mouse0) && !hasClickedThisFrame) {
            // if it is the 10th frame, do the thing
            if (Time.frameCount % 10 == 0) {
                Log("Mouse held down", Logger.Color.GREEN, this);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                int layerMask = LayerMask.GetMask("ExcludeFromMovementClicks");
                layerMask = ~layerMask;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
                    if (hit.collider.gameObject.CompareTag("Ground")) {
                        var point = hit.point;
                        point.y = transform.position.y;

                        // Uncomment to also spawn click prefab when holding down the mouse 
                        // Instantiate(m_ClickAnimPrefab, new Vector3(point.x, 0.2f, point.z), Quaternion.identity);

                        m_SelectedChampion.SetMouseHitPoint(point);
                    }

                    if (hit.collider.gameObject.CompareTag("Enemy") ||
                        hit.collider.gameObject.CompareTag("KitegirlGrenade")) {
                        m_SelectedChampion.OnAutoAttack(hit.collider);
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            m_SelectedChampion.OnAbility(KeyCode.Mouse1);
        }

        hasClickedThisFrame = false;
    }

    private void HandleAbilityClicks() {
        // If Q, W, E or R is pressed, call the m_SelectedChampion.OnAbility() method and pass in the correct KeyCode

        if (Input.GetKeyDown(KeyCode.Q)) {
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
        if (isPaused) return;
        HandleMouseClicks();
        HandleAbilityClicks();
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

    private void OnChampionDamageTakenEvent(ChampionDamageTakenEvent e) {
        EventBus<UpdateResourceBarEvent>.Raise(new UpdateResourceBarEvent("Health",
            m_SelectedChampion.GetCurrentHealth(), m_SelectedChampion.GetMaxHealth()));
    }

    private void OnChampionHealthRegenerated(ChampionHealthRegenerated e) {
        EventBus<UpdateResourceBarEvent>.Raise(new UpdateResourceBarEvent("Health",
            m_SelectedChampion.GetCurrentHealth(), m_SelectedChampion.GetMaxHealth()));
    }

    private void OnChampionLevelUp(ChampionLevelUpEvent e) {
        isPaused = true;
        m_SelectedChampion.Stop();
    }

    private void OnChampionAbilityChosen(ChampionAbilityChosenEvent e) {
        isPaused = false;
    }

    public Champion GetCurrentlySelectedChampion() {
        return m_SelectedChampion;
    }
}