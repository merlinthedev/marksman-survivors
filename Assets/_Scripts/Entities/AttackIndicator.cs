using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class AttackIndicator : MonoBehaviour {

    private enum Shape { CIRCLE, SQUARE, TRIANGLE };
    [SerializeField] private Shape shape;
    private GameObject fill;
    [SerializeField] private bool isFill = true;
    public float castTime = 1f;
    public float growthFactor = 0.01f;
    [SerializeField] private UnityEngine.Color fillColor = UnityEngine.Color.red;
    public Vector3 direction;
    public float distance;

    private void Start() {
        if (isFill) return;
        if(shape == Shape.CIRCLE) {
            GetComponentsInChildren<AttackIndicator>()[1].growthFactor = 1 / (castTime / 0.02f);
        }
        else if(shape == Shape.SQUARE) {
            GetComponentsInChildren<AttackIndicator>()[1].growthFactor = 1 / (castTime / 0.02f);

        }
        else if(shape == Shape.TRIANGLE) {
            Debug.Log("Triangle");
        }
        
        
    }

    private void FixedUpdate() {
        if (!isFill) return;
        if(shape == Shape.CIRCLE) {
            if(transform.localScale.x < transform.parent.localScale.x) {
                transform.localScale = new Vector3(transform.localScale.x + growthFactor, transform.localScale.y, transform.localScale.z + growthFactor);
            }
            else {
                Destroy(gameObject);
            }
        }
        else if(shape == Shape.SQUARE) {
            if(transform.localScale.x < transform.parent.localScale.x) {
                transform.localScale = new Vector3(transform.localScale.x + growthFactor, transform.localScale.y, transform.localScale.z);
            }
            else {
                Destroy(gameObject);
            }
        }
        else if(shape == Shape.TRIANGLE) {
            Debug.Log("Triangle");
        }
    }
}
