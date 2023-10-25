using _Scripts.Entities;
using System.Collections.Generic;

namespace _Scripts.BuffsDebuffs {
    public interface IDebuffable : IDamageable {
        List<Debuff> Debuffs { get; }
        void ApplyDebuff(Debuff debuff);
        void RemoveDebuff(Debuff debuff);
        void CheckDebuffsForExpiration();
    }
}