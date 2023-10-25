using _Scripts.EventBus;
using _Scripts.Inventory.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI {
    public class UIMerchantPanelItem : MonoBehaviour {
        [SerializeField] private Image panelImage;
        [SerializeField] private TMP_Text panelText;
        [SerializeField] private Button panelButton;

        private Item item;

        public void OnButtonClick() {
            Debug.Log("Clicked " + item);

            EventBus<MerchantItemBuyRequestEvent>.Raise(new MerchantItemBuyRequestEvent(item) {
                panelButton = panelButton
            });
        }

        public Image GetPanelImage() {
            return panelImage;
        }

        public TMP_Text GetPanelText() {
            return panelText;
        }

        public Button GetPanelButton() {
            return panelButton;
        }

        public Item GetItem() {
            return item;
        }

        public void SetItem(Item item) {
            this.item = item;
        }
    }
}