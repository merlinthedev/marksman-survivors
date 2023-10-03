using System.Collections.Generic;
using EventBus;
using Inventory.Items;
using UnityEngine;

namespace UI {
    public class MerchantPanelController : MonoBehaviour {
        [SerializeField] private UIMerchantPanelItem merchantPanelItemPrefab;
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject controlPanel;
        private List<Item> items;

        private void OnEnable() {
            EventBus<MerchantInteractEvent>.Subscribe(OnMerchantInteract);
        }

        private void OnDisable() {
            EventBus<MerchantInteractEvent>.Unsubscribe(OnMerchantInteract);
        }

        public void ExitMerchantPanel() {
            EventBus<MerchantExitEvent>.Raise(new MerchantExitEvent());
            OnMerchantExit();
        }

        private void Start() {
            mainPanel.SetActive(false);
        }

        private void PopulatePanel() {
            for (int i = 0; i < 5; i++) {
                UIMerchantPanelItem merchantPanelItem = Instantiate(merchantPanelItemPrefab, controlPanel.transform);
                merchantPanelItem.GetPanelText().text = "Item " + i;
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
            items = e.Items;

            PopulatePanel();
        }

        private void OnMerchantExit() {
            HidePanel();
        }
    }
}