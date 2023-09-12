using UnityEngine;

namespace UI {
    public class ClickAnimator : MonoBehaviour
    {
        [SerializeField] private GameObject m_CirclePrefab;

        public void OnLineFinish() {
            Instantiate(m_CirclePrefab, new Vector3(transform.position.x, 0.001f, transform.position.z), Quaternion.Euler(90, 0, 0));
            Destroy(gameObject);
        }

        public void OnCircleFinish() {
            Destroy(gameObject);
        }
    }
}
