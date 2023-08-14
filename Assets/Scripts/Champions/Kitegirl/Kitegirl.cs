using UnityEngine;
using Util;

public class Kitegirl : Champion {

    [SerializeField] private Bullet m_BulletPrefab;

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

                // Invoke(nameof(SetCanMove(true)), .1f);
                Utilities.InvokeDelayed(() => { SetCanMove(true); }, 0.1f, this);
                m_Rigidbody.velocity = Vector3.zero;

                // shoot 3 bullets in burst mode

                ShootBullet_Recursive(true, point);


            }
        }
    }

    public override void OnAbility(KeyCode keyCode) {
        AAbility ability = this.m_Abilities.Find(ability => ability.GetKeyCode() == keyCode);
        if (ability != null) {
            ability.OnUse();
        }
    }

    protected override void Update() {
        base.Update(); // this is important, even tho the editor says it's not...
    }

    private int recurseCount = 0;
    private int maxRecurseCount = 3;

    private void ShootBullet_Recursive(bool shouldCallRecursive, Vector3 target) {
        recurseCount++;

        Vector3 randomBulletSpread = new Vector3(
            Random.Range(-0.1f, 0.1f),
            0,
            Random.Range(-0.1f, 0.1f)
        );

        Bullet bullet = Instantiate(m_BulletPrefab, transform.position, Quaternion.identity);
        bullet.SetTarget(target + randomBulletSpread);
        bullet.SetDamage(m_Damage);

        if (shouldCallRecursive && recurseCount < maxRecurseCount) {
            Utilities.InvokeDelayed(() => { ShootBullet_Recursive(true, target); }, 0.05f, this);
        } else {
            recurseCount = 0;
        }
    }
}