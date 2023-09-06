using System.Collections.Generic;

public interface IDebuffable : IDamageable {
    List<Debuff> Debuffs { get; }
    void ApplyDebuff(Debuff debuff);
    void RemoveDebuff(Debuff debuff);
    void CheckDebuffsForExpiration();
}