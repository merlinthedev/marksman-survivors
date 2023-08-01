using System;
using UnityEngine;

public class Movement : MonoBehaviour {
    private Collider m_Collider;
    private Rigidbody m_Rigidboby;

    private Vector3 hitPoint;

    [SerializeField] private float m_MovementSpeed = 5f;

    private void Start() {
        m_Collider = GetComponent<Collider>();
        m_Rigidboby = GetComponent<Rigidbody>();

        if (m_Collider == null || m_Rigidboby == null) {
            throw new Exception("Missing collider or rigidbody");
        }

    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            Debug.Log("Mouse 1 down");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits) {
                if (hit.collider.gameObject.tag == "Ground") {
                    var point = hit.point;
                    point.y = transform.position.y;
                    hitPoint = point;
                    break;
                }
            }

        }

        if (hitPoint != Vector3.zero) {
            Move();
        }
    }


    private void Move() {
        Vector3 direction = hitPoint - transform.position;

        if (direction.magnitude < 0.1f) {
            hitPoint = Vector3.zero;
            m_Rigidboby.velocity = Vector3.zero;
            return;
        }

        m_Rigidboby.velocity = direction.normalized * m_MovementSpeed;

    }
}