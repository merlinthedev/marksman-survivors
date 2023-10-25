using _Scripts.EventBus;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {
    [SerializeField] private int amount = 1;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            EventBus<AddGoldEvent>.Raise(new AddGoldEvent(amount));
            Destroy(gameObject);
        }
    }
}
