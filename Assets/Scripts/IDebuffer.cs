using System.Collections.Generic;

public interface IDebuffer {
    List<IDamageable> AffectedEntities { get; set; }
}