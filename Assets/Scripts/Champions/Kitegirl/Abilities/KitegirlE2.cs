using UnityEngine;

public class KitegirlE2 : AAbility {
    [SerializeField] private KitegirlSmokescreen m_SmokescreenPrefab = null;
    [SerializeField] private float m_DashRange = 15f;

    public override void OnUse() {
        if (IsOnCooldown()) return;


        // TODO: Spawn smokescreen
        this.m_LastUseTime = Time.time;
        KitegirlSmokescreen kitegirlSmokescreen = Instantiate(m_SmokescreenPrefab,
            m_Champion.transform.position, Quaternion.identity);
        kitegirlSmokescreen.OnThrow(m_Champion);

        m_Champion.Stop();
        (m_Champion as Kitegirl)?.SmokeScreenPushBack(m_DashRange);
    }
}