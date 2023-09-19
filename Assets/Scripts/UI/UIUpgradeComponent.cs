using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class UIUpgradeComponent : MonoBehaviour {
        [SerializeField] private TMP_Text upgradeText;
        [SerializeField] private Image bannerImage;

        public TMP_Text GetTextComponent() {
            return upgradeText;
        }

        public Image GetBannerImage() {
            return bannerImage;
        }
    }
}