using System.Collections.Generic;

public interface IDamageable {
    bool IsBurning { get; set; }
    bool IsFragile { get; set; }
    float FragileStacks { get; set; }
    float LastFragileApplyTime { get; set; }

    List<Debuff> Debuffs { get; }

    void TakeFlatDamage(float damage);
    void TakeBurnDamage(float damage, float interval, float time);
    void ApplyDebuff(Debuff debuff);
    void RemoveDebuff(Debuff debuff);
}