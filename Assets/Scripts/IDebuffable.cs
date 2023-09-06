using System.Collections.Generic;

public interface IDebuffable {
    List<Debuff> Debuffs { get; }
    void ApplyDebuff(Debuff debuff);
    void RemoveDebuff(Debuff debuff);
}