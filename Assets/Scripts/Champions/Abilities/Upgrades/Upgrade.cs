using UnityEngine;
using Logger = Util.Logger;

namespace Champions.Abilities.Upgrades {
    public class Upgrade : MonoBehaviour {
        private bool unlocked = false;

        public void OnApply() {
            Logger.Log("An upgrade was applied!", Logger.Color.RED, this);
        }

        public bool IsUnlocked() {
            return unlocked;
        }
    }
}