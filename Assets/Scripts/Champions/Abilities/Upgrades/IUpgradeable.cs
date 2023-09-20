using System.Collections.Generic;

namespace Champions.Abilities.Upgrades {
    public interface IUpgradeable {
        void OnUpgrade(Upgrade upgrade);
    }
}