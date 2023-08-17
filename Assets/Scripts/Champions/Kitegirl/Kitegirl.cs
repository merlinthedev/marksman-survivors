using UnityEngine;
using Util;

public class Kitegirl : Champion {

    [SerializeField] private KitegirlBullet m_BulletPrefab;
    [SerializeField] private GameObject m_AnimationController;

    private bool m_AutoAttackShouldChain = false;
    private bool m_IsDashing = false;
    private bool m_HasUltimateActive = false;

    private int m_RecurseCount = 0;
    private int m_MaxRecurseCount = 3;
    private int m_MaxChainCount { get { return m_MaxRecurseCount * 3; } }
    private int m_CurrentChainCount = 0;

    [SerializeField] private float m_DashSpeed = 20f;

    public override void OnAutoAttack() {
        if (!CanAttack) return;
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
                this.m_MouseHitPoint = transform.position; // ???
                m_LastAttackTime = Time.time;

                AAbility ability = this.m_Abilities.Find(ability => ability.GetKeyCode() == KeyCode.E);
                if (ability.IsOnCooldown()) {
                    ability.DeductFromCooldown(1f);
                }

                // Invoke(nameof(SetCanMove(true)), .1f);
                Utilities.InvokeDelayed(() => { SetCanMove(true); }, 0.1f, this);
                m_Rigidbody.velocity = Vector3.zero;

                // shoot 3 bullets in burst mode

                ShootBullet_Recursive(true, point);
                m_AnimationController.Attack();

            }
        }
    }

    public override void OnAbility(KeyCode keyCode) {
        AAbility ability = this.m_Abilities.Find(ability => ability.GetKeyCode() == keyCode);

        if (ability != null) {
            ability.OnUse();
        } else {
            Debug.Log("Ability not found");
        }
    }

    public void ActivateUltimate() {
        m_HasUltimateActive = true;

        SetMovementDebuff(0.3f);
        SetDamageMultiplier(2f); // 2x damage

        this.m_HasAttackCooldown = false;
    }

    public void DeactivateUltimate() {
        m_HasUltimateActive = false;

        ResetMovementMultiplier();
        ResetDamageMultiplier();

        this.m_HasAttackCooldown = true;
    }

    protected override void OnMove() {
        if (!m_IsDashing) {
            base.OnMove();
        } else {
            this.m_Rigidbody.velocity = GetCurrentMovementDirection() * (m_DashSpeed * GetCurrentMovementMultiplier());
        }
    }

    protected override void Update() {
        base.Update(); // this is important, even tho the editor says it's not...
    }

    private void Start() {
        foreach (AAbility ability in m_Abilities) {
            ability.Hook(this);
        }
    }


    private void ShootBullet_Recursive(bool shouldCallRecursive, Vector3 target) {
        m_RecurseCount++;

        Vector3 randomBulletSpread = new Vector3(
            Random.Range(-0.1f, 0.1f),
            0,
            Random.Range(-0.1f, 0.1f)
        );

        // ABullet aBullet = Instantiate(mABulletPrefab, transform.position, Quaternion.identity);
        // aBullet.SetShouldChain(m_AutoAttackShouldChain);
        // aBullet.SetTarget(target + randomBulletSpread);
        // aBullet.SetDamage(m_Damage);

        KitegirlBullet bullet = Instantiate(m_BulletPrefab, transform.position, Quaternion.identity);
        bullet.SetShouldChain(m_AutoAttackShouldChain);
        bullet.SetTarget(target + randomBulletSpread);
        bullet.SetDamage(this.m_Damage * GetDamageMultiplier());

        if (m_AutoAttackShouldChain) {
            m_CurrentChainCount++;
            // Debug.Log("Chain count: " + m_CurrentChainCount + " / " + m_MaxChainCount + "");
            if (m_CurrentChainCount >= m_MaxChainCount) {
                m_CurrentChainCount = 0;
                m_AutoAttackShouldChain = false;
                // Debug.Log("Chain count reset");
            }
        }

        if (shouldCallRecursive && m_RecurseCount < m_MaxRecurseCount) {
            Utilities.InvokeDelayed(() => { ShootBullet_Recursive(true, target); }, 0.05f, this);
        } else {
            m_RecurseCount = 0;
        }
    }


    public void SetAutoAttackChain(bool b) {
        m_AutoAttackShouldChain = b;
    }

    public void SetIsDashing(bool p0) {
        m_IsDashing = p0;
    }


}