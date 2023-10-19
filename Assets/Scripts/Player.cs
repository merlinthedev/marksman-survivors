using System.Collections.Generic;
using Champions;
using Core;
using Enemies;
using Entities;
using EventBus;
using Interactable;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;

public class Player : Core.Singleton.Singleton<Player> {
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

    private GameObject currentFocus;

    private Inventory.Inventory inventory;

    private Enemy lastHoveredEnemy;
    private float lastEnemyHoverTime = 0f;

    [Header("FORGIVENESS CLICKS")]
    [SerializeField] private float maxForgivenessDistance = 2.1f;

    [SerializeField] private float forgivenessTime = 0.15f;

    private void OnEnable() {
        // excludedContexts.Add(this);

        EventBus<EnemyStartHoverEvent>.Subscribe(OnEnemyStartHover);
        EventBus<EnemyStopHoverEvent>.Subscribe(OnEnemyStopHover);

        EventBus<InteractableStartHoverEvent>.Subscribe(OnInteractableStartHover);
        EventBus<InteractableStopHoverEvent>.Subscribe(OnInteractableStopHover);

        EventBus<ChampionHealthRegenerated>.Subscribe(OnChampionHealthRegenerated);
        EventBus<ChampionDamageTakenEvent>.Subscribe(OnChampionDamageTakenEvent);

        EventBus<ChampionManaRegenerated>.Subscribe(OnChampionManaRegenerated);

        EventBus<LoadSceneEvent>.Subscribe(LoadScene);


        EventBus<GamePausedEvent>.Subscribe(OnGamePaused);
        EventBus<GameResumedEvent>.Subscribe(OnGameResumed);
    }

    private void OnDisable() {
        EventBus<EnemyStartHoverEvent>.Unsubscribe(OnEnemyStartHover);
        EventBus<EnemyStopHoverEvent>.Unsubscribe(OnEnemyStopHover);

        EventBus<InteractableStartHoverEvent>.Unsubscribe(OnInteractableStartHover);
        EventBus<InteractableStopHoverEvent>.Unsubscribe(OnInteractableStopHover);

        EventBus<ChampionHealthRegenerated>.Unsubscribe(OnChampionHealthRegenerated);
        EventBus<ChampionDamageTakenEvent>.Unsubscribe(OnChampionDamageTakenEvent);

        EventBus<ChampionManaRegenerated>.Unsubscribe(OnChampionManaRegenerated);


        EventBus<LoadSceneEvent>.Unsubscribe(LoadScene);


        EventBus<GamePausedEvent>.Unsubscribe(OnGamePaused);
        EventBus<GameResumedEvent>.Unsubscribe(OnGameResumed);
    }

    private void Start() {
        SetDefaultCursorTexture();

        inventory = new Inventory.Inventory();
    }

    private void HandleMouseClicks() {
        HandleMoveClick();
        if (Time.time > lastEnemyHoverTime + forgivenessTime) {
            HandleMoveHoldClick();
        }

        HandleAttackMove();


        hasClickedThisFrame = false;

        if (Input.GetKeyDown(KeyCode.Escape)) {
            EventBus<ToggleMenuEvent>.Raise(new ToggleMenuEvent("settings"));
        }

        if (Input.GetKeyDown(KeyCode.Equals)) {
            EventBus<ToggleMenuEvent>.Raise(new ToggleMenuEvent("cheats"));
        }

        if (Input.GetKeyDown(KeyCode.Tab) && !isPaused) {
            EventBus<ShowLevelUpPanelEvent>.Raise(new ShowLevelUpPanelEvent());
        }
    }

    private void HandleAttackMove() {
        if (Input.GetKeyDown(KeyCode.A) && !isPaused) {
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
                    point.y = transform.position.y;

                    IDamageable damageable = DamageableManager.GetInstance().GetClosestDamageable(point);

                    if (damageable != null) {
                        selectedChampion.OnAutoAttack(damageable);
                        var x = Instantiate(clickAnimPrefab, new Vector3(point.x, 0.2f, point.z), Quaternion.identity);
                        x.GetComponent<Renderer>().material.color = Color.red;

                        if (damageable is Enemy) {
                            RemoveFocus();

                            damageable.GetTransform().GetComponent<Renderer>().material.SetInt("_Focus", 1);
                            damageable.GetTransform().GetComponent<Enemy>().focusAnim = true;
                            currentFocus = damageable.GetTransform().gameObject;
                        }
                    }
                } else {
                    IDamageable damageable = hit.collider.gameObject.GetComponent<IDamageable>();
                    if (damageable != null) {
                        if (damageable as Player != this) {
                            selectedChampion.OnAutoAttack(damageable);

                            RemoveFocus();

                            damageable.GetTransform().GetComponent<Renderer>().material.SetInt("_Focus", 1);
                            damageable.GetTransform().GetComponent<Enemy>().focusAnim = true;
                            currentFocus = damageable.GetTransform().gameObject;
                        }
                    }
                }
            }
        }
    }

    private void HandleMoveHoldClick() {
        if (Input.GetKey(KeyCode.Mouse0) && !hasClickedThisFrame && !isPaused) {
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

                        // selectedChampion.SetMouseHitPoint(point);
                        selectedChampion.RequestMovement(point);
                    }

                    if (hit.collider.gameObject.CompareTag("Enemy") ||
                        hit.collider.gameObject.CompareTag("KitegirlGrenade")) {
                        IDamageable damageable = hit.collider.gameObject.GetComponent<IDamageable>();
                        selectedChampion.OnAutoAttack(damageable);
                    }
                }
            }
        }
    }

    private void HandleMoveClick() {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !isPaused) {
            // check if the mouse is on a canvas object
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
                // Debug.LogError("Mouse is over a UI element, not moving" +
                //                UnityEngine.EventSystems.EventSystem.current.transform.name);
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

                    Enemy enemy = EnemyManager.GetInstance().GetClosestEnemy(point);
                    if (Time.time < lastEnemyHoverTime + forgivenessTime && lastHoveredEnemy != null) {
                        if (enemy != null) {
                            float distance = (enemy.transform.position - point).magnitude;

                            if (distance < maxForgivenessDistance) {
                                selectedChampion.OnAutoAttack(enemy);

                                SetFocus(enemy);
                                return;
                            }
                        }
                    }


                    selectedChampion.RequestMovement(point);
                }

                if (hit.collider.gameObject.CompareTag("Enemy") ||
                    hit.collider.gameObject.CompareTag("KitegirlGrenade")) {
                    IDamageable damageable = hit.collider.gameObject.GetComponent<IDamageable>();
                    selectedChampion.OnAutoAttack(damageable);

                    SetFocus(damageable);
                } else {
                    RemoveFocus();
                }

                //If we clicked on an interactable object, call the OnInteract method
                var interactable = hit.collider.gameObject.GetComponent<IInteractable>();
                if (interactable != null) {
                    float distance = Vector3.Distance(hit.collider.gameObject.transform.position,
                        gameObject.transform.position);

                    if (distance > 5f) {
                        selectedChampion.RequestMovement(hit.collider.gameObject.transform.position, 5f,
                            () => interactable.OnInteract());
                        // Logger.Log("distance is too big, moving...", Logger.Color.RED, this);
                    } else {
                        // Logger.Log("distance is not too big, interacting...", Logger.Color.RED, this);
                        interactable.OnInteract();
                    }
                }
            }
        }
    }

    private void SetFocus(IDamageable damageable) {
        RemoveFocus();

        if (!(damageable is Enemy)) return;

        damageable.GetTransform().GetComponent<Renderer>().material.SetInt("_Focus", 1);
        damageable.GetTransform().GetComponent<Enemy>().focusAnim = true;
        currentFocus = damageable.GetTransform().gameObject;
    }

    private void RemoveFocus() {
        if (currentFocus != null) {
            currentFocus.GetComponent<Renderer>().material.SetInt("_Focus", 0);
        }
    }

    private void HandleAbilityClicks() {
        if (isPaused) return;
        // If Q, W, E or R is pressed, call the m_SelectedChampion.OnAbility() method and pass in the correct KeyCode

        if (Input.GetKeyDown(KeyCode.Q)) {
            if (selectedChampion.GetAbilities()[0] != null) {
                selectedChampion.OnAbility(selectedChampion.GetAbilities()[0]);
            }
        }

        if (Input.GetKeyDown(KeyCode.W)) {
            if (selectedChampion.GetAbilities()[1] != null) {
                selectedChampion.OnAbility(selectedChampion.GetAbilities()[1]);
            }
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            if (selectedChampion.GetAbilities()[2] != null) {
                selectedChampion.OnAbility(selectedChampion.GetAbilities()[2]);
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            if (selectedChampion.GetAbilities()[3] != null) {
                selectedChampion.OnAbility(selectedChampion.GetAbilities()[3]);
            }
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            if (selectedChampion.GetAbilities()[4] != null) {
                selectedChampion.OnAbility(selectedChampion.GetAbilities()[4]);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            selectedChampion.OnDash();
        }
    }

    private void Update() {
        HandleMouseClicks();
        HandleAbilityClicks();
    }

    private void SetDefaultCursorTexture() {
        Cursor.SetCursor(m_CursorTexture, Vector2.zero, CursorMode.Auto);
    }

    private void OnEnemyStartHover(EnemyStartHoverEvent e) {
        Cursor.SetCursor(m_AttackCursorTexture, Vector2.zero, CursorMode.Auto);
        lastEnemyHoverTime = Time.time;
        lastHoveredEnemy = e.enemy;
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

    private void OnChampionManaRegenerated(ChampionManaRegenerated e) {
        EventBus<UpdateResourceBarEvent>.Raise(new UpdateResourceBarEvent("Mana",
            selectedChampion.GetChampionStatistics().CurrentMana, selectedChampion.GetChampionStatistics().MaxMana));
    }

    private void OnGamePaused(GamePausedEvent e) {
        isPaused = true;
        selectedChampion.Stop();
    }

    private void OnGameResumed(GameResumedEvent e) {
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


    public Inventory.Inventory GetInventory() {
        return inventory;
    }
}