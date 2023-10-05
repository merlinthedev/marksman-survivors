using System.Collections.Generic;
using EventBus;
using Interactable.NPC;
using Inventory.Items;
using UnityEngine;

namespace UI {
    public class MerchantPanelController : MonoBehaviour {
        [SerializeField] private UIMerchantPanelItem merchantPanelItemPrefab;
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject controlPanel;
        private List<Item> items;
        private Merchant merchant;

        private void OnEnable() {
            EventBus<MerchantInteractEvent>.Subscribe(OnMerchantInteract);
        }

        private void OnDisable() {
            EventBus<MerchantInteractEvent>.Unsubscribe(OnMerchantInteract);
        }

        public void ExitMerchantPanel() {
            EventBus<MerchantExitEvent>.Raise(new MerchantExitEvent(merchant));
            OnMerchantExit();
        }

        private void Start() {
            mainPanel.SetActive(false);
        }

        private void PopulatePanel() {
            for (int i = 0; i < items.Count; i++) {
                UIMerchantPanelItem merchantPanelItem = Instantiate(merchantPanelItemPrefab, controlPanel.transform);
                merchantPanelItem.GetPanelText().SetText(items[i].GetName() + " - " + items[i].GetPrice());
                merchantPanelItem.SetItem(items[i]);
            }

            mainPanel.SetActive(true);
        }

        private void HidePanel() {
            foreach (Transform child in controlPanel.transform) {
                Destroy(child.gameObject);
            }

            mainPanel.SetActive(false);
        }

        private void OnMerchantInteract(MerchantInteractEvent e) {
            items = e.items;
            merchant = e.merchant;

            PopulatePanel();
        }

        private void OnMerchantExit() {
            HidePanel();
        }
    }
}