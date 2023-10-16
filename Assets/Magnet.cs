using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour {
    private Vector3 target = Vector3.zero;
    private bool magnetized = false;
    private GameObject player = null;
    [SerializeField] private Rigidbody rb;

    private void Start() {
        rb = transform.root.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")) {
            player = other.gameObject;
            magnetized = true;
        }
    }
    

    private void MoveParent() {
        float speed = Vector3.Distance(transform.root.position, target) * 0.04f + 0.05f;
        Debug.Log(speed);
        transform.root.position = Vector3.MoveTowards(transform.root.position, new Vector3(target.x, transform.root.position.y, target.z), speed);

    }

    private void FixedUpdate() {
        if (magnetized) {
            target = player.transform.position;
            MoveParent();
        }
    }
}
