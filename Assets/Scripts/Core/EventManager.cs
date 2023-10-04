using System.Collections.Generic;
using Interactable.NPC;
using EventBus;
using UnityEngine;

namespace Core {
    public class EventManager : MonoBehaviour {
        private static EventManager instance;

        private List<Merchant> activeMerchants = new();
        [SerializeField] private GameObject merchantPrefab;

        private void OnEnable() {
            EventBus<MerchantExitEvent>.Subscribe(OnMerchantExit);
        }

        private void OnDisable() {
            EventBus<MerchantExitEvent>.Unsubscribe(OnMerchantExit);
        }

        private void OnMerchantExit(MerchantExitEvent e) {
            RemoveMerchant(e.merchant);
            Destroy(e.merchant.gameObject);
        }

        private void RemoveMerchant(Merchant merchant) {
            activeMerchants.Remove(merchant);
            Destroy(merchant.gameObject);
        }

        private void Start() {
            if (instance != null) {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        public static EventManager GetInstance() {
            return instance;
        }
    }
}