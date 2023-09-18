using UnityEngine;
using UnityEngine.Serialization;

namespace UI {
    public class ClickAnimator : MonoBehaviour {
        [SerializeField] private GameObject circlePrefab;

        public void OnLineFinish() {
            Instantiate(circlePrefab, new Vector3(transform.position.x, 0.001f, transform.position.z),
                Quaternion.Euler(90, 0, 0));
            Destroy(gameObject);
        }

        public void OnCircleFinish() {
            Destroy(gameObject);
        }
    }
}