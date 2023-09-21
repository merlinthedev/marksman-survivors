using Champions;
using Enemy;
using EventBus;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {
    [Header("Stats")]
    [SerializeField] private Texture2D m_CursorTexture, m_AttackCursorTexture, m_InteractCursorTexture;

    [SerializeField] private LayerMask attackLayerMask;

    [Header("UI")]
    [SerializeField] private Image healthBar;

    [SerializeField] private Champion selectedChampion;

    [Header("Other")]
    [SerializeField] private GameObject clickAnimPrefab;

    private bool isPaused = false;
    private bool firstMove = true;
    private bool hasClickedThisFrame = false;

    private Inventory.Inventory inventory;

    private void OnEnable() {
        // excludedContexts.Add(this);

        EventBus<EnemyStartHoverEvent>.Subscribe(OnEnemyStartHover);
        EventBus<EnemyStopHoverEvent>.Subscribe(OnEnemyStopHover);

        EventBus<InteractableStartHoverEvent>.Subscribe(OnInteractableStartHover);
        EventBus<InteractableStopHoverEvent>.Subscribe(OnInteractableStopHover);

        EventBus<ChampionHealthRegenerated>.Subscribe(OnChampionHealthRegenerated);
        EventBus<ChampionDamageTakenEvent>.Subscribe(OnChampionDamageTakenEvent);

        EventBus<LoadSceneEvent>.Subscribe(LoadScene);

        EventBus<UILevelUpPanelOpenEvent>.Subscribe(OnLevelUpPanelOpen);
        EventBus<UILevelUpPanelClosedEvent>.Subscribe(OnLevelUpPanelClosed);
    }

    private void OnDisable() {
        EventBus<EnemyStartHoverEvent>.Unsubscribe(OnEnemyStartHover);
        EventBus<EnemyStopHoverEvent>.Unsubscribe(OnEnemyStopHover);

        EventBus<InteractableStartHoverEvent>.Unsubscribe(OnInteractableStartHover);
        EventBus<InteractableStopHoverEvent>.Unsubscribe(OnInteractableStopHover);

        EventBus<ChampionHealthRegenerated>.Unsubscribe(OnChampionHealthRegenerated);
        EventBus<ChampionDamageTakenEvent>.Unsubscribe(OnChampionDamageTakenEvent);

        EventBus<LoadSceneEvent>.Unsubscribe(LoadScene);

        EventBus<UILevelUpPanelOpenEvent>.Unsubscribe(OnLevelUpPanelOpen);
        EventBus<UILevelUpPanelClosedEvent>.Unsubscribe(OnLevelUpPanelClosed);
    }

    private void Start() {
        SetDefaultCursorTexture();

        inventory = new Inventory.Inventory();
    }

    private void HandleMouseClicks() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            // check if the mouse is on a canvas object
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            int layerMask = LayerMask.GetMask("ExcludeFromMovementClicks");
            layerMask = ~layerMask;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
                if (hit.collider.gameObject.CompareTag("Ground")) {
                    hasClickedThisFrame = true;
                    var point = hit.point;
                    Instantiate(clickAnimPrefab, new Vector3(point.x, 0.2f, point.z), Quaternion.identity);
                    point.y = transform.position.y;

                    if (firstMove) {
                        EnemyManager.GetInstance().SetShouldSpawn(true);
                        firstMove = false;
                    }

                    selectedChampion.SetMouseHitPoint(point);
                }

                if (hit.collider.gameObject.CompareTag("Enemy") ||
                    hit.collider.gameObject.CompareTag("KitegirlGrenade")) {
                    selectedChampion.OnAutoAttack(hit.collider);
                }

                //If we clicked on an interactable object, call the OnInteract method
                var interactable = hit.collider.gameObject.GetComponent<IInteractable>();
                if (interactable != null) {
                    interactable.OnInteract();
                }
            }
        }

        if (Input.GetKey(KeyCode.Mouse0) && !hasClickedThisFrame) {
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
                return;
            }

            // if it is the 10th frame, do the thing
            if (Time.frameCount % 10 == 0) {
                // Log("Mouse held down", Logger.Color.GREEN, this);
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

                        selectedChampion.SetMouseHitPoint(point);
                    }

                    if (hit.collider.gameObject.CompareTag("Enemy") ||
                        hit.collider.gameObject.CompareTag("KitegirlGrenade")) {
                        selectedChampion.OnAutoAttack(hit.collider);
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            selectedChampion.OnAbility(KeyCode.Mouse1);
        }

        hasClickedThisFrame = false;
    }

    private void HandleAbilityClicks() {
        // If Q, W, E or R is pressed, call the m_SelectedChampion.OnAbility() method and pass in the correct KeyCode

        if (Input.GetKeyDown(KeyCode.Q)) {
            selectedChampion.OnAbility(KeyCode.Q);
        }

        if (Input.GetKeyDown(KeyCode.W)) {
            selectedChampion.OnAbility(KeyCode.W);
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            selectedChampion.OnAbility(KeyCode.E);
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            selectedChampion.OnAbility(KeyCode.R);
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

    private void OnInteractableStartHover(InteractableStartHoverEvent e) {
        Cursor.SetCursor(m_InteractCursorTexture, Vector2.zero, CursorMode.Auto);
    }

    private void OnInteractableStopHover(InteractableStopHoverEvent e) {
        SetDefaultCursorTexture();
    }

    private void OnChampionDamageTakenEvent(ChampionDamageTakenEvent e) {
        EventBus<UpdateResourceBarEvent>.Raise(new UpdateResourceBarEvent("Health",
            selectedChampion.GetCurrentHealth(), selectedChampion.GetMaxHealth()));
    }

    private void OnChampionHealthRegenerated(ChampionHealthRegenerated e) {
        EventBus<UpdateResourceBarEvent>.Raise(new UpdateResourceBarEvent("Health",
            selectedChampion.GetCurrentHealth(), selectedChampion.GetMaxHealth()));
    }

    private void OnLevelUpPanelOpen(UILevelUpPanelOpenEvent e) {
        isPaused = true;
        selectedChampion.Stop();
    }

    private void OnLevelUpPanelClosed(UILevelUpPanelClosedEvent e) {
        isPaused = false;
    }

    public Champion GetCurrentlySelectedChampion() {
        return selectedChampion;
    }

    #region Scene Management

    public void LoadScene(LoadSceneEvent e) {
        SceneManager.LoadScene(e.sceneName);
    }

    #endregion
}