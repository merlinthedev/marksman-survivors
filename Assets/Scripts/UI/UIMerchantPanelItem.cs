using EventBus;
using Inventory.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class UIMerchantPanelItem : MonoBehaviour {
        [SerializeField] private Image panelImage;
        [SerializeField] private TMP_Text panelText;
        [SerializeField] private Button panelButton;

        private Item item;

        public void OnButtonClick() {
            Debug.Log("Clicked " + item);

            EventBus<MerchantItemBuyRequestEvent>.Raise(new MerchantItemBuyRequestEvent(item));

            panelButton.interactable = false;
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