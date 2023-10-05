using System;
using System.Collections;
using System.Collections.Generic;
using Interactable.NPC;
using EventBus;
using UnityEngine;
using Util;
using Logger = Util.Logger;
using Random = UnityEngine.Random;

namespace Core {
    public class EventManager : MonoBehaviour {
        private static EventManager instance;

        private List<Merchant> activeMerchants = new();
        private Player player;
        [SerializeField] private GameObject merchantPrefab;

        [SerializeField] private float spawnInterval = 1f;
        [SerializeField] private float spawnChanceIncrease = 0.005f;
        [SerializeField] private float initialSpawnChance = 0.01f;

        private float currentMerchantSpawnChance;

        private bool canSpawnMerchant = true;

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
            player = Player.GetInstance();

            currentMerchantSpawnChance = initialSpawnChance;

            StartCoroutine(SpawnMerchant());
        }

        private IEnumerator SpawnMerchant() {
            yield return new WaitForSeconds(spawnInterval);

            if (!canSpawnMerchant) {
                Logger.Log("Cannot spawn merchant yet", Logger.Color.RED, this);
                yield break;
            }
            
            // 0-1, 0.001
            
            // Q: What is the chance of Random.value to be smaller than 0.0001 in %?
            
            

            if (Random.value < currentMerchantSpawnChance) {
                // spawn a merchant
                Vector3 spawnPoint = Utilities.GetRandomPointInTorus(player.transform.position, 50f);
                GameObject merchantObject = Instantiate(merchantPrefab, spawnPoint, Quaternion.Euler(0, 45, 0));
                Merchant merchant = merchantObject.GetComponent<Merchant>();
                if (merchant == null) {
                    Logger.LogError("Merchant prefab does not have a Merchant component", this);
                    throw new Exception("Merchant prefab does not have a Merchant component");
                }

                activeMerchants.Add(merchant);
                Logger.Log("Merchant spawned at location: " + spawnPoint, Logger.Color.RED, this);
                canSpawnMerchant = false;
                StartCoroutine(AfterMerchantSpawn());
            } else {
                Logger.Log("Merchant did not spawn, increasing spawn chance", this);
                currentMerchantSpawnChance += spawnChanceIncrease;

                StartCoroutine(SpawnMerchant());
            }
        }


        private IEnumerator AfterMerchantSpawn() {
            currentMerchantSpawnChance = initialSpawnChance;
            yield return new WaitForSeconds(30f);

            canSpawnMerchant = true;
        }

        public static EventManager GetInstance() {
            return instance;
        }
    }
}