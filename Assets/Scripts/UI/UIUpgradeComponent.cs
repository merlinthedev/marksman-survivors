using TMPro;
using UnityEngine;

namespace UI {
    public class UIUpgradeComponent : MonoBehaviour {
        [SerializeField] private TMP_Text upgradeText;

        public TMP_Text GetTextComponent() {
            return upgradeText;
        }
    }
}