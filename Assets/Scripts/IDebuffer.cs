using System.Collections.Generic;

public interface IDebuffer {
    List<IDebuffable> AffectedEntities { get; set; }
    void CheckDebuffsForExpiration();
}