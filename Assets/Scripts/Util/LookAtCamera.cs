using UnityEngine;

public class LookAtCamera : MonoBehaviour {
    [SerializeField] private Transform target;
    [SerializeField] private Quaternion offset;

    void Start() {
        //set camera to the main camera transform
        target = Camera.main.transform;
        transform.LookAt(target);
        transform.rotation = transform.rotation * offset;
    }

}