using System.Collections;
using UnityEngine;

namespace _Scripts.Champions.Kitegirl.Entities {
    public class HitboxTimer : MonoBehaviour {
        [SerializeField] float time = 0.5f;
        // Start is called before the first frame update
        void Start() {
            StartCoroutine(Timer());
        }

        private IEnumerator Timer()
        {
            yield return new WaitForSeconds(time);
            Destroy(gameObject);
        }
    }
}
